using DoAn.Data;

namespace DoAn.Service
{
    public interface IBookingRepository
    {
        Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkin, DateTime checkout);
        Task<Product?> GetProductAsync(int id);
        Task<User?> GetUserAsync(Guid id);
        Task AddBookingAsync(Booking booking);
        Task<Booking?> GetBookingAsync(int bookingId);
        Task<BookingDetail?> GetBookingDetailAsync(int bookingId, int roomId);
        Task SaveChangesAsync();
        Task<Booking?> GetFullBookingByIdAsync(int bookingId);
        Task<List<Booking>> GetBookingsByCustomerIdAsync(Guid customerId);
        Task<bool> DeleteBookingAsync(int bookingId);
    }
}
