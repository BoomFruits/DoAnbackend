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
        public async Task<IActionResult> GetAllBooking(string mode = "today", DateTime? from = null, DateTime? to = null)
        {
            var result = await _bookingService.GetAllBookingAsync(mode, from, to);
            return Ok(result);
        }
        [HttpGet("my-bookings")]
        public async Task<IActionResult> GetMyBookings()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Người dùng không hợp lệ" });
            }
            var bookings = await _bookingService.GetMyBookingsAsync(userId);
            return Ok(bookings);
        }
        [HttpDelete("cancel/{id}")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var (success, message) = await _bookingService.CancelBookingAsync(id);
            if(!success)
                return BadRequest(new { message });
            return Ok(new { message });
        }
        [HttpPost("checkin/{bookingId}/{roomId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CheckIn(int bookingId, int roomId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var staffId))
            {
                return Unauthorized(new { message = "Người dùng không hợp lệ" });
            }
            var (success, message) = await _bookingService.CheckInAsync(bookingId, roomId, staffId);
            if(!success)
                return BadRequest(new {message });
            return Ok(new { message});
        }
        [HttpPost("checkout/{bookingId}/{roomId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CheckOut(int bookingId, int roomId)
        {
            var (success,message) = await _bookingService.CheckOutAsync(bookingId, roomId);
            if(!success)
                return BadRequest(new { message });
            return Ok(new { message });
        }
        [HttpDelete("delete/{bookingId}")]
        public async Task<IActionResult> DeleteBooking(int bookingId)
        {
            var (success,message) = await _bookingService.DeleteBookingAsync(bookingId);
            if(!success)
                return BadRequest(new { message });
            return Ok(new { message });
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBooking(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Customer).Include(b => b.BookingDetails)
                .ThenInclude(d => d.Room)
                .Include(b => b.ServiceDetails)
                    .ThenInclude(s => s.Product)
                .FirstOrDefaultAsync(b => b.Id == id);
            if (booking == null)
                return NotFound();
            var result = new BookingDTO
            {
                Id = booking.Id,
                BookingDate = booking.BookingDate,
                PaymentMethod = booking.PaymentMethod,
                Note = booking.Note,
                IsPaid = booking.IsPaid,
                PaymentDate = booking.PaymentDate,
                TotalPrice = booking.TotalPrice,
                Status = booking.status,
                UserName = booking.Customer?.Username,
                UserEmail = booking.Customer?.Email,
                CheckinDate = booking.BookingDetails.Min(d => d.CheckinDate).ToString("yyyy-MM-dd"),
                CheckoutDate = booking.BookingDetails.Max(d => d.CheckoutDate).ToString("yyyy-MM-dd"),

                Details = booking.BookingDetails.Select(d => new BookingDetailDTO
                {
                    RoomId = d.RoomId,
                    Room_No = d.Room.Room_No,
                    CheckinDate = d.CheckinDate,
                    CheckoutDate = d.CheckoutDate,
                    Price = d.Price,
                    IsCheckedIn = d.IsCheckedIn,
                    IsCheckedOut = d.IsCheckedOut,
                    RoomNote = d.RoomNote,
                    TotalAmount = d.TotalAmount,
                    Services = booking.ServiceDetails
                        .Where(s => s.RoomId == d.RoomId && s.BookingId == booking.Id)
                        .Select(s => new ServiceDTO
                        {
                            Title = s.Product.Title,
                            Amount = s.Amount,
                            Price = s.Price
                        }).ToList()
                }).ToList()
            };

            return Ok(result);
        }
    }
}
