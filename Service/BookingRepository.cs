using DoAn.Data;
using Microsoft.EntityFrameworkCore;

namespace DoAn.Service
{
    public class BookingRepository : IBookingRepository
    {
        private readonly DbBookingContext _context;
        public BookingRepository(DbBookingContext context)
        {
            _context = context;
        }
        public async Task AddBookingAsync(Booking booking)
        {
           await _context.Bookings.AddAsync(booking);
        }

        public async Task<Booking?> GetFullBookingByIdAsync(int bookingId)
        {
            return await _context.Bookings
                .Include(b => b.BookingDetails)
                .ThenInclude(d => d.Room)
                .Include(b => b.ServiceDetails)
                .ThenInclude(s => s.Product)
                .FirstOrDefaultAsync(b => b.Id == bookingId);
        }
        public async Task<List<Booking>> GetBookingsByCustomerIdAsync(Guid customerId)
        {
            return await _context.Bookings
                .Include(b => b.BookingDetails)
                    .ThenInclude(d => d.Room)
                .Include(b => b.ServiceDetails)
                    .ThenInclude(s => s.Product)
                .Where(b => b.CustomerId == customerId)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();
        }
        public Task<Booking?> GetBookingAsync(int bookingId)
              =>  _context.Bookings.FindAsync(bookingId).AsTask();

        public Task<BookingDetail?> GetBookingDetailAsync(int bookingId, int roomId)
            => _context.BookingDetails.FirstOrDefaultAsync(d => d.BookingId == bookingId && d.RoomId == roomId);
        public async Task<Product?> GetProductAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<User?> GetUserAsync(Guid id)
        {
           return await _context.Users.FindAsync(id);
        }

        public async Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkin, DateTime checkout)
        {
            return !await _context.BookingDetails.Include(d => d.Booking)
                .AnyAsync(d =>
                    d.RoomId == roomId &&
                    d.Room.IsAvailable == true &&
                    !d.IsCheckedIn && !d.IsCheckedOut &&
                    checkin < d.CheckoutDate && checkout > d.CheckinDate);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteBookingAsync(int bookingId)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if(booking == null)
            {
                return false; 
            }
            await _context.Bookings
                .Where(b => b.Id == bookingId)
                .ExecuteDeleteAsync();
            return true;
        }
    }
}
