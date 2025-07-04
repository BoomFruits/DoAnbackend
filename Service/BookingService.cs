using DoAn.Data;
using DoAn.DTO;
using Microsoft.EntityFrameworkCore;

namespace DoAn.Service
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepo;
        private readonly IEmailService _emailService;
        private readonly IUserService _userService;
        public BookingService(IBookingRepository bookingRepo, IEmailService emailService, IUserService userService)
        {
            _bookingRepo = bookingRepo;
            _emailService = emailService;
            _userService = userService;
        }
        public async Task<(bool Success, string Message, int? bookingId)> CreateBookingAsync(CreateBookingDTO dto, Guid userId)
        {
            var booking = new Booking
            {
                CustomerId = userId,
                BookingDate = DateTime.Now,
                Note = dto.Note ?? "",
                PaymentMethod = dto.PaymentMethod,
                status = 0,
                TotalPrice = 0
            };
            double total = 0;

            foreach (var detail in dto.Details)
            {
                if (!await _bookingRepo.IsRoomAvailableAsync(detail.RoomId, detail.CheckinDate, detail.CheckoutDate))
                {
                    return (false, $"Phòng {detail.RoomId} đã được đặt trong khoảng thời gian từ " + detail.CheckinDate + "-> " + detail.CheckoutDate, null);
                }

                total += detail.Price;
                booking.BookingDetails.Add(new BookingDetail
                {
                    RoomId = detail.RoomId,
                    CheckinDate = detail.CheckinDate,
                    CheckoutDate = detail.CheckoutDate,
                    RoomNote = detail.RoomNote ?? "",
                    Price = detail.Price
                });

                foreach (var service in detail.Services)
                {
                    var product = await _bookingRepo.GetProductAsync(service.serviceId);
                    if (product == null || product.StockQuantity < service.Amount)
                        return (false, $"Không đủ tồn kho cho dịch vụ Id = {service.serviceId}", null);

                    product.StockQuantity -= service.Amount;
                    total += service.Amount * service.Price;

                    booking.ServiceDetails.Add(new ServiceDetail
                    {
                        RoomId = detail.RoomId,
                        ProductId = service.serviceId,
                        Amount = service.Amount,
                        Price = service.Price,
                        CustomerId = userId
                    });
                }
            }

            booking.TotalPrice = (int)total;

            var customer = await _bookingRepo.GetUserAsync(userId);
            if (customer == null) return (false, "Không tìm thấy người dùng", null);

            await _bookingRepo.AddBookingAsync(booking);
            await _bookingRepo.SaveChangesAsync();

            var fullBooking = await _bookingRepo.GetFullBookingByIdAsync(booking.Id);
            await _emailService.SendBookingConfirmationAsync(customer.Email, customer.Username, fullBooking);

            return (true, "Đặt phòng thành công", booking.Id);
        }

        async Task<(bool Success, string Message)> IBookingService.CheckInAsync(int bookingId, int roomId, Guid staffId)
        {
            var detail = await _bookingRepo.GetBookingDetailAsync(bookingId, roomId);
            if (detail == null)
                return (false, "Không tìm thấy chi tiết đặt phòng");

            if (detail.IsCheckedIn)
                return (false, "Đã checked in");

            detail.IsCheckedIn = true;

            var booking = await _bookingRepo.GetBookingAsync(bookingId);
            if (booking == null)
                return (false, "Không tìm thấy đơn đặt phòng");

            booking.StaffId = staffId;
            booking.status = 1;
            booking.IsPaid = true;
            booking.PaymentDate = DateTime.Now;

            await _bookingRepo.SaveChangesAsync();

            return (true, "Checked in thành công");
        }
        public async Task<(bool Success, string Message)> CheckOutAsync(int bookingId, int roomId)
        {
            var booking = await _bookingRepo.GetBookingAsync(bookingId);
            if (booking == null)
                return (false, "Không tìm thấy đơn đặt phòng");

            var detail = await _bookingRepo.GetBookingDetailAsync(bookingId, roomId);
            if (detail == null)
                return (false, "Phòng không hợp lệ");

            if (!detail.IsCheckedIn)
                return (false, "Yêu cầu check-in trước khi check-out");

            if (detail.IsCheckedOut)
                return (true, "Phòng đã được check-out");

            detail.IsCheckedOut = true;


            if (booking.BookingDetails.All(d => d.IsCheckedOut))
            {
                booking.status = 3; // 3 = hoàn tất
            }

            await _bookingRepo.SaveChangesAsync();

            return (true, "Checked out thành công");
        }

        public async Task<(bool success, string messsage)> CancelBookingAsync(int bookingId)
        {
            var booking = await _bookingRepo.GetBookingAsync(bookingId);
            if (booking == null) return (false,"Không tìm thấy đơn đặt phòng");

            if (booking.status == 2) return (false,"Đã huỷ");
            if (booking.status == 3) return (false,"Đã hoàn thành");

            booking.status = 2; // 2 = Cancelled
            await _bookingRepo.SaveChangesAsync();
            return (true, "Huỷ đặt phòng thành công");
        }
       public async Task<(bool Success, string Message)> DeleteBookingAsync(int bookingId)
        {
            var booking = await _bookingRepo.GetBookingAsync(bookingId); 
            if (booking == null) return (false, "Không tìm thấy đơn đặt phòng");

            if (await _bookingRepo.DeleteBookingAsync(bookingId))
            {
                await _bookingRepo.SaveChangesAsync();
                return (true, "Xoá đơn đặt phòng thành công");
            }
            else
            {
                return (false, "Không thể xoá đơn đặt phòng");
            }
        }
        public async Task<List<BookingDTO>> GetMyBookingsAsync(Guid customerId)
        {
            var bookings = await _bookingRepo.GetBookingsByCustomerIdAsync(customerId);

            return bookings.Select(b => new BookingDTO
            {
                Id = b.Id,
                BookingDate = b.BookingDate,
                PaymentMethod = b.PaymentMethod,
                Note = b.Note,
                IsPaid = b.IsPaid,
                PaymentDate = b.PaymentDate,
                TotalPrice = b.TotalPrice,
                Status = b.status,
                Details = b.BookingDetails.Select(d => new BookingDetailDTO
                {
                    RoomId = d.RoomId,
                    Room_No = d.Room.Room_No,
                    CheckinDate = d.CheckinDate,
                    CheckoutDate = d.CheckoutDate,
                    Price = d.Price,
                    IsCheckedIn = d.IsCheckedIn,
                    IsCheckedOut = d.IsCheckedOut,
                    RoomNote = d.RoomNote,
                    Services = b.ServiceDetails
                        .Where(s => s.RoomId == d.RoomId)
                        .Select(s => new ServiceDTO
                        {
                            Title = s.Product.Title,
                            Amount = s.Amount,
                            Price = s.Price
                        }).ToList()
                }).ToList()
            }).ToList();
        }
    }

}
