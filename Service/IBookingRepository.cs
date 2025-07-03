using DoAn.Data;

namespace DoAn.Service
{
    public interface IBookingRepository
    {
        Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkin, DateTime checkout);
        Task<Product?> GetProductAsync(int id);
        Task<User?> GetUserAsync(Guid id);
        Task AddBookingAsync(Booking booking);
        Task SaveChangesAsync();
        Task<Booking?> GetFullBookingByIdAsync(int bookingId);
    }
}
