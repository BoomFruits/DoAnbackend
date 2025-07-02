using DoAn.Data;
using DoAn.DTO;
namespace DoAn.Service
{
    public interface IServiceDetailService
    {
        Task<bool> AddServiceToRoomAsync(CreateServiceDetailDTO dto, Guid userId);
        Task<IEnumerable<ServiceDetail>> GetServicesByBookingDetailAsync(int bookingId);
        Task DeleteServiceDetailAsync(int serviceId);
    }
}
