using DoAn.Data;
using DoAn.DTO;
using DoAn.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IUserService _userService;
        private readonly DbBookingContext _context;
        public UsersController(IUserService userService, IPasswordHasher<User> passwordHasher, DbBookingContext context)
        {
            _userService = userService;
            _passwordHasher = passwordHasher;
            _context = context;
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            var created = await _userService.CreateUserAsync(user);
            return CreatedAtAction(nameof(GetById), new { id = created.id }, created);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
                => Ok(await _userService.GetAllUsersAsync());
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return false;

            _context.Bookings.RemoveRange(user.CustomerBookings);
            _context.Bookings.RemoveRange(user.StaffBookings);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id,[FromBody] UpdateUserDTO dto)
        {
            var updated = await _userService.UpdateUserAsync(id, dto);
            return updated == null ? NotFound() : Ok(updated);
        }
        [Authorize]
        [HttpPost("{id}/change-password")]
        public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordDTO dto)
        {
            try
            {
                var userDb = await _userService.ChangePasswordAsync(id, dto);
                return Ok(new { passwordMessage = "Đổi mật khẩu thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { passwordMessage = ex.Message });
            }
        }
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO dto)
        {
            var result = await _userService.ResetPasswordAsync(dto.Email);
            if (!result)
                return NotFound("Email not found");

            return Ok(new {message = "New password have been sent to your email"});
        }
    }
}
