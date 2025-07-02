using DoAn.Data;

namespace DoAn.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByUsernameAsync(string username);
        Task AddAsync(User user);
        void Update(User user);
        Task DeleteAsync(Guid id);
        Task SaveChangesAsync();
        Task<User?> GetUserByEmail(String email);
    }
}
