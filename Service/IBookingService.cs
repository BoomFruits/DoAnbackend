using DoAn.DTO;
using System;

namespace DoAn.Service
{
    public interface IBookingService
    {
        Task<(bool Success, string Message,int? bookingId)> CreateBookingAsync(CreateBookingDTO dto, Guid userId);
        Task<(bool Success,string Message)> CheckInAsync(int bookingId, int roomId, Guid staffId);
        Task<(bool Success, string Message)> CheckOutAsync(int bookingId, int roomId);
        Task<(bool success,string messsage)> CancelBookingAsync(int bookingId);
        Task<(bool Success, string Message)> DeleteBookingAsync(int bookingId);
        Task<List<BookingDTO>> GetMyBookingsAsync(Guid customerId);
    }
}

