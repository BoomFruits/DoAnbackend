using DoAn.Data;
using DoAn.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Net.WebSockets;

namespace DoAn.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles ="Admin")]
    public class StatisticController : Controller
    {
        //private readonly IBookingService _bookingService;
        //private readonly IRoomService _roomService;
        //public StatisticController(IBookingService bookingService, IRoomService roomService)
        //{
        //    _bookingService = bookingService;
        //    _roomService = roomService;
        //}
        private readonly DbBookingContext _context;
        public StatisticController(DbBookingContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetStatistics(string mode = "today", DateTime? from = null, DateTime? to = null)
        {
            DateTime today = DateTime.Today;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);

            // Lọc booking theo mode
            IQueryable<Booking> filteredBookings = _context.Bookings;

            if (mode == "today")
            {
                filteredBookings = filteredBookings.Where(b => b.BookingDate.Date == today);
            }
            else if (mode == "month")
            {
                filteredBookings = filteredBookings.Where(b => b.BookingDate >= firstDayOfMonth);
            }
            else if (mode == "custom" && from.HasValue && to.HasValue)
            {
                filteredBookings = filteredBookings.Where(b => b.BookingDate.Date >= from.Value.Date && b.BookingDate.Date <= to.Value.Date);
            }
            // Dữ liệu tổng quan Các thống kê theo ngày và tháng
            var totalBookings = await filteredBookings.CountAsync();
            var totalRevenue = await filteredBookings
                .Where(b => b.IsPaid)
                .SumAsync(b => (decimal?)b.TotalPrice) ?? 0;
            var todayRevenue = await _context.Bookings
                .Where(b => b.BookingDate.Date == today && b.IsPaid)
                .SumAsync(b => (decimal?)b.TotalPrice) ?? 0;
            var successCount = await filteredBookings.CountAsync(b => b.status != 2); // status != canceled
            var canceledCount = await filteredBookings.CountAsync(b => b.status == 2);
            var todayBookings = await _context.Bookings.Where(b => b.BookingDate == today).CountAsync();
            var monthBookings = await _context.Bookings.CountAsync(b => b.BookingDate >= firstDayOfMonth);
            var todayCheckins = await _context.Bookings.Where(b => b.BookingDetails.Any(d => d.IsCheckedIn && d.CheckinDate.Date == today.Date)).CountAsync();
            var monthCheckins = await _context.BookingDetails.CountAsync(d => d.IsCheckedIn && d.CheckinDate >= firstDayOfMonth);
            var todayCheckouts = await _context.Bookings.Where(b => b.BookingDetails.Any(d => d.IsCheckedOut && d.CheckoutDate.Date == today.Date)).CountAsync();
            var monthCheckouts = await _context.BookingDetails.CountAsync(d => d.IsCheckedOut && d.CheckoutDate >= firstDayOfMonth);

            var userCount = await _context.Users.CountAsync();
            var activeRooms = await _context.Rooms.CountAsync(r => r.IsAvailable);

            // Top phòng theo số lượt đặt ( lọc custom )
            var topRooms = await filteredBookings
                .SelectMany(b => b.BookingDetails)
                .GroupBy(d => d.RoomId)
                .OrderByDescending(g => g.Count())
                .Take(5)
                .Select(g => new
                {
                    RoomId = g.Key,
                    Count = g.Count(),  
                    RoomNo = g.First().Room.Room_No
                })
                .ToListAsync();
            // Top dịch vụ theo tổng số lượng dùng ( lọc custom )
            var topServices = await filteredBookings
                .SelectMany(s => s.ServiceDetails)
                .GroupBy(s => s.ProductId)
                .OrderByDescending(g => g.Sum(x => x.Amount))
                .Take(5)
                .Select(g => new
                {
                    ServiceName = g.First().Product.Title,
                    TotalUsed = g.Sum(x => x.Amount)
                })
                .ToListAsync();

            // Doanh thu theo tháng
            var monthlyRevenue = await _context.Bookings
                .Where(b => b.IsPaid)
                .GroupBy(b => b.BookingDate.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Revenue = g.Sum(b => b.TotalPrice)
                })
                .ToListAsync();

            // Lượt đặt theo tháng
            var monthlyBookings = await _context.Bookings
                .GroupBy(b => b.BookingDate.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            return Ok(new
            {
                totalBookings,
                totalRevenue,
                todayRevenue,
                successCount,
                canceledCount,
                todayBookings,
                todayCheckins,
                monthCheckins,
                todayCheckouts,
                monthCheckouts,
                monthBookings,
                userCount,
                activeRooms,
                topRooms,
                topServices,
                monthlyRevenue,
                monthlyBookings
            });
        }
        //[HttpGet("summary")]
        //public async Task<IActionResult> GetDashboardSummary()
        //{
        //    var totalRevenue = await _context.Bookings.Where(b => b.IsPaid).SumAsync(b => b.TotalPrice);
        //    var totalBookings = await _context.Bookings.CountAsync();
        //    var totalRooms = await _context.Rooms.CountAsync();
        //    var activeRooms = await _context.BookingDetails.Where(d => d.IsCheckedIn && !d.IsCheckedOut).CountAsync();
        //    var todayCheckins = await _context.BookingDetails.CountAsync(d => d.CheckinDate.Date == DateTime.Today);
        //    var todayCheckouts = await _context.BookingDetails.CountAsync(d => d.CheckoutDate.Date == DateTime.Today);

        //    return Ok(new
        //    {
        //        totalRevenue,
        //        totalBookings,
        //        totalRooms,
        //        activeRooms,
        //        todayCheckins,
        //        todayCheckouts
        //    });
        //}
    }
}
