using DoAn.DTO;

namespace DoAn.Service
{
    public interface IBookingService
    {
        Task<(bool Success, string Message,int? bookingId)> CreateBookingAsync(CreateBookingDTO dto, Guid userId);
    }
}

