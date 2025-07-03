using DoAn.Data;
using DoAn.DTO;
using DoAn.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace DoAn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookingController : ControllerBase
    {
        private readonly DbBookingContext _context;
        private readonly IEmailService _emailService;
        private readonly IBookingService _bookingService;

        public BookingController(DbBookingContext context, IEmailService emailService, IBookingService bookingService)
        {
            _context = context;
            _emailService = emailService;
            _bookingService = bookingService;
        }

        //[HttpPost]
        //public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDTO dto)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            var errors = ModelState.Values
        //                .SelectMany(v => v.Errors)
        //                .Select(e => e.ErrorMessage)
        //                .ToList();
        //            return BadRequest(new { message = "Dữ liệu không hợp lệ", errors });
        //        }
        //        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        //        if(string.IsNullOrEmpty(userId))  return BadRequest(new { message = "User chưa xác thực" });
        //        // Check room availability
        //        foreach (var detail in dto.Details)
        //        {
        //            bool isConflict = await _context.BookingDetails
        //                .Include(b => b.Booking)
        //                .AnyAsync(d =>  
        //                    d.RoomId == detail.RoomId &&
        //                    d.Room.IsAvailable == true &&
        //                    d.IsCheckedIn == false && d.IsCheckedOut == false &&
        //                    detail.CheckinDate < d.CheckoutDate &&
        //                    detail.CheckoutDate > d.CheckinDate
        //                );

        //            if (isConflict)
        //            {
        //                return BadRequest(new { message = $"Phòng {detail.room_No} is already booked during the selected time." });
        //            }
        //        }
        //        var booking = new Booking
        //        {
        //            CustomerId = Guid.Parse(userId),
        //            StaffId = null,
        //            BookingDate = DateTime.Now,
        //            Note = dto.Note ?? "",
        //            PaymentMethod = dto.PaymentMethod,           
        //            status = 0, // pending , payment status = 1 , done status = 2 , cancel = 3
        //            TotalPrice = 0
        //        };
        //        double total = 0;
        //        foreach (var detail in dto.Details)
        //        {
        //            total += detail.Price; 
        //            booking.BookingDetails.Add(new BookingDetail
        //            {
        //                RoomId = detail.RoomId,            
        //                CheckinDate = detail.CheckinDate,
        //                CheckoutDate = detail.CheckoutDate,
        //                RoomNote = detail.RoomNote ?? "",
        //                Price = detail.Price
        //            });
        //            foreach(var service in detail.Services)
        //            {
        //                total += service.Quantity * service.Price;
        //                var product = await _context.Products.FindAsync(service.ServiceId);
        //                if (product == null || product.StockQuantity >= service.Quantity)
        //                {
        //                    product.StockQuantity -= service.Quantity;
        //                }
        //                else
        //                {
        //                    return BadRequest(new { message = $"Không đủ tồn kho cho dịch vụ {service.ServiceId}" });
        //                }
        //                booking.ServiceDetails.Add(new ServiceDetail
        //                {
        //                    RoomId = service.RoomId,
        //                    ProductId = service.ServiceId,
        //                    Amount = service.Quantity,
        //                    Price = service.Price,
        //                    CustomerId = Guid.Parse(userId),
        //                });
        //            }
        //        }
        //        booking.TotalPrice = (int)total;
        //        var customer = await _context.Users.FindAsync(booking.CustomerId);
        //        if (customer != null)
        //        {
        //            _context.Bookings.Add(booking);
        //            await _context.SaveChangesAsync();
        //            var fullBooking = await _context.Bookings.Include(b => b.BookingDetails)
        //                .ThenInclude(d => d.Room)
        //                .Include(b => b.ServiceDetails).ThenInclude(sd => sd.Product)
        //                .FirstOrDefaultAsync(b => b.Id == booking.Id);
        //            await _emailService.SendBookingConfirmationAsync(customer.Email, customer.Username, fullBooking);
        //        }

        //        return Ok(new { message = "Đặt phòng thành công",bookingId = booking.Id });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { message = "Lỗi", detail = ex.Message });
        //    }
        //}
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDTO dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { message = "Dữ liệu không hợp lệ", errors });
            }
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdStr, out var userId))
                return BadRequest(new { message = "User chưa xác thực" });

            var result = await _bookingService.CreateBookingAsync(dto, userId);
            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message, bookingId = result.bookingId });
        }
        [HttpGet("get_all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllBooking()
        {
            var bookings = await _context.Bookings
       .Include(b => b.Customer)
       .Include(b => b.BookingDetails)
           .ThenInclude(d => d.Room)
       .Select(b => new
       {
           b.Id,
           UserEmail = b.Customer.Email,
           UserName = b.Customer.Username,
           b.TotalPrice,
           b.status,
           b.PaymentDate,
           b.IsPaid,
           StaffName = b.Staff != null ? b.Staff.Username : null,
           bookingDate = b.BookingDate,
           b.PaymentMethod,
           b.Note,
           CheckinDate = b.BookingDetails.Min(d => d.CheckinDate),
           CheckoutDate = b.BookingDetails.Max(d => d.CheckoutDate),
           details = b.BookingDetails.Select(d => new
           {
               BookingId = b.Id,
               d.RoomId,
               d.Room.Room_No,
               d.CheckinDate,
               d.IsCheckedIn,
               d.CheckoutDate,
               d.IsCheckedOut,
               d.Price,
               d.Room.Room_Name,
               d.RoomNote,
               Services = b.ServiceDetails
                            .Where(s => s.RoomId == d.RoomId)
                            .Select(sd => new {
                                sd.Product.Title,
                                sd.Amount,
                                sd.Price
                            })
           })

       })
       .ToListAsync();
            return Ok(bookings);
        }
        [HttpGet("my-bookings")]
        public async Task<IActionResult> GetMyBookings()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Người dùng không hợp lệ" });
            }

            var bookings = await _context.Bookings
                .Include(b => b.BookingDetails)
                    .ThenInclude(d => d.Room)
                .Where(b => b.CustomerId == userId)
                .OrderByDescending(b => b.BookingDate)
                .Select(b => new
                {
                    b.Id,
                    b.BookingDate,
                    b.PaymentMethod,
                    b.Note,
                    b.IsPaid,
                    b.PaymentDate,
                    b.TotalPrice,
                    b.status,
                    Details = b.BookingDetails.Select(d => new
                    {
                        d.RoomId,
                        d.Room.Room_No,
                        d.CheckinDate,
                        d.IsCheckedIn,
                        d.CheckoutDate,
                        d.IsCheckedOut,
                        d.Price,
                        d.RoomNote,

                        Services = b.ServiceDetails
                            .Where(s => s.RoomId == d.RoomId)
                            .Select(sd => new {
                                sd.Product.Title,
                                sd.Amount,
                                sd.Price
                            })
                    })
                })
                .ToListAsync();

            return Ok(bookings);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBooking(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Staff)
                .Include(b => b.BookingDetails)
                    .ThenInclude(d => d.Room)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
                return NotFound();
            return Ok(new
            {
                BookingId = booking.Id,
                PaymentMethod = booking.PaymentMethod,
                CustomerName = booking.Customer.Username,
                TotalPrice = booking.TotalPrice,
                Rooms = booking.BookingDetails.Select(d => new {
                    Room = d.Room.Room_No,
                    Checkin = d.CheckinDate,
                    Checkout = d.CheckoutDate,
                    Price = d.Price
                })
            });
        }
        [HttpDelete("cancel/{id}")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();

            if (booking.status == 2) return BadRequest(new { message = "Đã huỷ." });
            if (booking.status == 3) return BadRequest(new { message = "Đã hoàn thành." });

            booking.status = 2; // 2 = Cancelled
            await _context.SaveChangesAsync();
            return Ok(new { message = "Huỷ đặt phòng." });
        }
        [HttpPost("checkin/{bookingId}/{roomId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CheckIn(int bookingId, int roomId)
        {
            var detail = await _context.BookingDetails
                .FirstOrDefaultAsync(d => d.BookingId == bookingId && d.RoomId == roomId);

            if (detail == null)
                return NotFound("Không tìm thấy chi tiết đặt phòng");

            if (detail.IsCheckedIn)
                return BadRequest("Đã checked in");

            detail.IsCheckedIn = true;
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null)
                return NotFound("Không tìm thấy đơn đặt phòng");
            else
            {//user payment
                booking.status = 1;
                booking.IsPaid = true;
                booking.PaymentDate = DateTime.Now;
            }
            await _context.SaveChangesAsync();

            return Ok(new { message = "Checked in thành công" });
        }
        [HttpPost("checkout/{bookingId}/{roomId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CheckOut(int bookingId, int roomId)
        {
            var booking = await _context.Bookings
                    .Include(b => b.BookingDetails)
                    .FirstOrDefaultAsync(b => b.Id == bookingId);
            if(booking == null)
                return NotFound("Không tìm thấy đơn đặt phòng");
            var detail = booking.BookingDetails.FirstOrDefault(d => d.RoomId == roomId);
            if (detail == null)
                return NotFound("Phòng không hợp lệ");
            if (!detail.IsCheckedIn)
                return BadRequest("Yêu cầu check-in trước khi check-out");
            if (detail.IsCheckedOut)
                return BadRequest("Đã checked out");
            detail.IsCheckedOut = true;
            await _context.SaveChangesAsync();
            if(booking.BookingDetails.All(d => d.IsCheckedOut))
            {
                booking.status = 3; // Set booking status to 'finished'
                await _context.SaveChangesAsync();
            }
            return Ok(new { message = "Checked out thành công" });
        }
        [HttpDelete("delete/{bookingId}")]
        public async Task<IActionResult> DeleteBooking(int bookingId)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);
            if (booking == null)
                return NotFound("không tìm thấy đơn đặt phòng");
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Xoá đơn đặt phòng thành công" });
        }
    }
}
