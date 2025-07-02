using DoAn.Data;
using Microsoft.EntityFrameworkCore;

namespace DoAn.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DbBookingContext _context;

        public UserRepository(DbBookingContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
            => await _context.Users.ToListAsync();

        public async Task<User?> GetByIdAsync(Guid id)
            => await _context.Users.FindAsync(id);

        public async Task<User?> GetByUsernameAsync(string username)
            => await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

        public async Task AddAsync(User user)
            => await _context.Users.AddAsync(user);

        public void  Update(User user)
        {
            _context.Users.Update(user);
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await GetByIdAsync(id);
            if (user != null)
                _context.Users.Remove(user);
        }

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }
    }
}
