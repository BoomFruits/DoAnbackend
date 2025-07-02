using DoAn.Data;
using DoAn.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace DoAn.Service
{
    public class ServiceDetailService : IServiceDetailService
    {
        private readonly DbBookingContext _context;

        public ServiceDetailService(DbBookingContext context)
        {
            _context = context;
        }
        public async Task<bool> AddServiceToRoomAsync(CreateServiceDetailDTO dto, Guid userId)
        {
            var bookingDetail = await _context.BookingDetails
                .Include(b => b.Booking)
                .FirstOrDefaultAsync(x => x.RoomId == dto.RoomId);

            if (bookingDetail == null)
                throw new Exception("Booking detail not found");

            var product = await _context.Products.FindAsync(dto.ServiceId);
            if (product == null || !product.IsAvailable)
                throw new Exception("Product not available");

            //var service = new ServiceDetail
            //{
            //    BookingId = dto.B,
            //    ProductId = dto.ProductId,
            //    CustomerId = userId,
            //    Amount = dto.Amount,
            //    Price = product.Price,
            //    BuyDate = DateTime.Now
            //};

            //_context.ServiceDetail.Add(service);
            //await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<ServiceDetail>> GetServicesByBookingDetailAsync(int bookingDetailId)
        {
            return await _context.ServiceDetail
                .Include(s => s.Product)
                .Where(x => x.BookingId == bookingDetailId)
                .ToListAsync();
        }
        [Authorize(Roles = "Admin")]
        public async Task DeleteServiceDetailAsync(int serviceId)
        {
            var service = await _context.ServiceDetail.FindAsync(serviceId);
            if (service != null)
            {
                _context.ServiceDetail.Remove(service);
                await _context.SaveChangesAsync();
            }
        }

    }
}
