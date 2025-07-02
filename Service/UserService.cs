using AutoMapper;
using DoAn.Data;
using DoAn.DTO;
using DoAn.Helpers;
using DoAn.Repositories;
using Microsoft.AspNetCore.Identity;

namespace DoAn.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IEmailService _emailService;
        public UserService(IUserRepository repo,IMapper mapper, IPasswordHasher<User> passwordHasher,IEmailService emailService)
            {
                _repo = repo;
                _mapper = mapper;
                _passwordHasher = passwordHasher;
            _emailService = emailService;
        }
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _repo.GetByUsernameAsync(username);
        }
        public async Task<User> CreateUserAsync(User user)
        {
            var existsUsername = await _repo.GetByUsernameAsync(user.Username);
            if (existsUsername != null)
                throw new Exception("Username đã tồn tại");
            var exitssEmail = await _repo.GetUserByEmail(user.Email);
            if (exitssEmail != null)
                throw new Exception("Email đã tồn tại");
            if(!string.IsNullOrWhiteSpace(user.Password))
            {
                user.Password = _passwordHasher.HashPassword(user, user.Password);
            }
            else
            {
                var defaultPassword = MyUtils.GenerateRandomPassword();
                user.Password = _passwordHasher.HashPassword(user, defaultPassword);
                await _emailService.SendEmailPwdDefault(user.Email, "Password Default",
               $"Your password default is: <strong>{defaultPassword}</strong>");
            }
                user.CreatedAt = DateTime.Now;
            await _repo.AddAsync(user);
            await _repo.SaveChangesAsync();
            return user;
        }

        public async Task<User?> UpdateUserAsync(Guid id, UpdateUserDTO dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return null;
            existing.Username = dto.Username;
            existing.PhoneNumber = dto.PhoneNumber;
            existing.Address = dto.Address;
            existing.Gender = dto.Gender;
            existing.RoleId = dto.RoleId;
            existing.UpdatedAt = DateTime.Now;
            _repo.Update(existing);
            await _repo.SaveChangesAsync();
            return existing;
        }
        public async Task<bool> DeleteUserAsync(Guid id)
        {
            await _repo.DeleteAsync(id);
            await _repo.SaveChangesAsync();
            return true;
        }

        public async Task<User?> GetUserByEmail(String email)
        {
            var user = await _repo.GetUserByEmail(email);
            return user;
        }

        public async Task<User?> ChangePasswordAsync(Guid id, ChangePasswordDTO dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                throw new Exception("User không tồn tại");

            if (string.IsNullOrWhiteSpace(existing.Password))
                throw new Exception("Tài khoản này chưa có mật khẩu. Vui lòng tạo mật khẩu trước.");

            var result = _passwordHasher.VerifyHashedPassword(existing, existing.Password, dto.OldPassword);
            if (result == PasswordVerificationResult.Failed)
                throw new Exception("Mật khẩu cũ không đúng");

            existing.Password = _passwordHasher.HashPassword(existing, dto.NewPassword);
            existing.UpdatedAt = DateTime.Now;
            _repo.Update(existing);
            await _repo.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> ResetPasswordAsync(string email)
        {
            var user = await _repo.GetUserByEmail(email);
            if (user == null) return false;
            var newPassword = MyUtils.GenerateRandomPassword();
            var hashedPassword = _passwordHasher.HashPassword(user, newPassword);
            user.Password = hashedPassword;
            user.UpdatedAt = DateTime.Now;
            _repo.Update(user);
            await _repo.SaveChangesAsync();
            await _emailService.SendEmailPwdDefault(email, "Password Reset",
                $"Your password has been reset. Your new password is: <strong>{newPassword}</strong>");
            return true;
        }

    }
}

