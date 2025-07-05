using DoAn.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace DoAn.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly DbBookingContext _context;

        public NotificationController(DbBookingContext context)
        {
            _context = context;
        }

        // lấy danh sách thông báo của người dùng
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Notification>>> GetUserNotifications(Guid userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Notification>>> GetAllNotificationsForAdmin()
        {
            var notifications = await _context.Notifications
                .Include(n => n.User).Where(n => !n.AdminRead)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return Ok(notifications);
        }
        // Đánh dấu đã đọc
        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkUserAsRead(int id)
        {
            var noti = await _context.Notifications.FindAsync(id);
            if (noti == null) return NotFound();

            noti.IsRead = true;
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/admin-read")]
        public async Task<IActionResult> MarkAdminNotificationAsRead(int id)
        {
            var noti = await _context.Notifications.FindAsync(id);
            if (noti == null) return NotFound();

            noti.AdminRead = true;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
