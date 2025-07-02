using DoAn.Data;
using DoAn.DTO;

namespace DoAn.Service
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(Guid id);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User> CreateUserAsync(User user);
        Task<User?> UpdateUserAsync(Guid id, UpdateUserDTO dto);
        Task<User?> ChangePasswordAsync(Guid id, ChangePasswordDTO dto);
        Task<bool> DeleteUserAsync(Guid id);
        Task<User?> GetUserByEmail(String email);
        Task<bool> ResetPasswordAsync(String email);
    }
}
