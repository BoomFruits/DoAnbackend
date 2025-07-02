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

        public BookingController(DbBookingContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(new { message = "Invalid data", errors });
                }
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if(string.IsNullOrEmpty(userId))  return BadRequest(new { message = "User not authenticated" });
                // Check room availability
                foreach (var detail in dto.Details)
                {
                    bool isConflict = await _context.BookingDetails
                        .Include(b => b.Booking)
                        .AnyAsync(d =>
                            d.RoomId == detail.RoomId &&
                            d.Room.IsAvailable == true &&
                            d.IsCheckedIn == false && d.IsCheckedOut == false &&
                            detail.CheckinDate < d.CheckoutDate &&
                            detail.CheckoutDate > d.CheckinDate
                        );

                    if (isConflict)
                    {
                        return BadRequest(new { message = $"Room ID {detail.RoomId} is already booked during the selected time." });
                    }
                }
                var booking = new Booking
                {
                    CustomerId = Guid.Parse(userId),
                    StaffId = null,
                    BookingDate = DateTime.Now,
                    Note = dto.Note ?? "",
                    PaymentMethod = dto.PaymentMethod,
                    status = 0, // pending , payment status = 1 , done status = 2 , cancel = 3
                    TotalPrice = 0
                };
                double total = 0;
                foreach (var detail in dto.Details)
                {
                    total += detail.Price; 
                    booking.BookingDetails.Add(new BookingDetail
                    {
                        RoomId = detail.RoomId,
                        CheckinDate = detail.CheckinDate,
                        CheckoutDate = detail.CheckoutDate,
                        RoomNote = detail.RoomNote ?? "",
                        Price = detail.Price
                    });
                    foreach(var service in detail.Services)
                    {
                        total += service.Quantity * service.Price;
                        var product = await _context.Products.FindAsync(service.ServiceId);
                        if (product == null || product.StockQuantity >= service.Quantity)
                        {
                            product.StockQuantity -= service.Quantity;
                        }
                        else
                        {
                            return BadRequest(new { message = $"Không đủ tồn kho cho dịch vụ {service.ServiceId}" });
                        }
                        booking.ServiceDetails.Add(new ServiceDetail
                        {
                            RoomId = service.RoomId,
                            ProductId = service.ServiceId,
                            Amount = service.Quantity,
                            Price = service.Price,
                            CustomerId = Guid.Parse(userId),
                        });
                    }
                }
                booking.TotalPrice = (int)total;
                var customer = await _context.Users.FindAsync(booking.CustomerId);
                if (customer != null)
                {
                    _context.Bookings.Add(booking);
                    await _context.SaveChangesAsync();
                    var fullBooking = await _context.Bookings.Include(b => b.BookingDetails)
                        .ThenInclude(d => d.Room)
                        .Include(b => b.ServiceDetails).ThenInclude(sd => sd.Product)
                        .FirstOrDefaultAsync(b => b.Id == booking.Id);
                    await _emailService.SendBookingConfirmationAsync(customer.Email, customer.Username, fullBooking);
                }

                return Ok(new { message = "Booking created successfully",bookingId = booking.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error msg", detail = ex.Message });
            }
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
           rooms = b.BookingDetails.Select(d => new
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
               d.RoomNote
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
                return Unauthorized(new { message = "Invalid user ID" });
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
                    }),
                    Services = b.ServiceDetails.Select(sd => new
                    {
                        sd.Product.Title,
                        sd.Amount,
                        sd.Price,
                        sd.BuyDate,
                        sd.RoomId
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

            if (booking.status == 2) return BadRequest(new { message = "Already canceled." });
            if (booking.status == 3) return BadRequest(new { message = "Already finished." });

            booking.status = 2; // 2 = Cancelled
            await _context.SaveChangesAsync();
            return Ok(new { message = "Booking canceled." });
        }
        [HttpPost("checkin/{bookingId}/{roomId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CheckIn(int bookingId, int roomId)
        {
            var detail = await _context.BookingDetails
                .FirstOrDefaultAsync(d => d.BookingId == bookingId && d.RoomId == roomId);

            if (detail == null)
                return NotFound("Booking detail not found");

            if (detail.IsCheckedIn)
                return BadRequest("Already checked in");

            detail.IsCheckedIn = true;
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null)
                return NotFound("Booking not found");
            else
            {//user payment
                booking.status = 1;
                booking.IsPaid = true;
                booking.PaymentDate = DateTime.Now;
            }
            await _context.SaveChangesAsync();

            return Ok(new { message = "Checked in successfully" });
        }
        [HttpPost("checkout/{bookingId}/{roomId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CheckOut(int bookingId, int roomId)
        {
            var booking = await _context.Bookings
                    .Include(b => b.BookingDetails)
                    .FirstOrDefaultAsync(b => b.Id == bookingId);
            if(booking == null)
                return NotFound("Booking not found");
            var detail = booking.BookingDetails.FirstOrDefault(d => d.RoomId == roomId);
            if (detail == null)
                return NotFound("Room not found in booking");
            if (!detail.IsCheckedIn)
                return BadRequest("Check-in required before check-out");
            if (detail.IsCheckedOut)
                return BadRequest("Already checked out");
            detail.IsCheckedOut = true;
            await _context.SaveChangesAsync();
            if(booking.BookingDetails.All(d => d.IsCheckedOut))
            {
                booking.status = 3; // Set booking status to 'finished'
                await _context.SaveChangesAsync();
            }
            return Ok(new { message = "Checked out successfully" });
        }
        [HttpDelete("delete/{bookingId}")]
        public async Task<IActionResult> DeleteBooking(int bookingId)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);
            if (booking == null)
                return NotFound("Booking not found");
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Deleted booking successfully" });
        }
    }
}
