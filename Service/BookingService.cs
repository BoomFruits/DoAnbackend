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
                    var product = await _bookingRepo.GetProductAsync(service.ServiceId);
                    if (product == null || product.StockQuantity < service.Quantity)
                        return (false, $"Không đủ tồn kho cho dịch vụ Id = {service.ServiceId}", null);

                    product.StockQuantity -= service.Quantity;
                    total += service.Quantity * service.Price;

                    booking.ServiceDetails.Add(new ServiceDetail
                    {
                        RoomId = service.RoomId,
                        ProductId = service.ServiceId,
                        Amount = service.Quantity,
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
    }
}
