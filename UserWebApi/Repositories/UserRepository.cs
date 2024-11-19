using Microsoft.EntityFrameworkCore;
using UserWebApi.Models;

namespace UserWebApi.Repositories
{
    public class UserRepository :GenericRepository<User>, IUserRepository
    {
        private readonly UserDbContext _dbContext;

        public UserRepository(UserDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _dbContext.Users
                .Include(u => u.Friends1)
                .Include(u => u.Friends2)
                .ToListAsync();
        }
        

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _dbContext.Users.SingleOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>> SearchUsersByNameAsync(string name)
        {
            return await _dbContext.Users
                .Where(u => u.Name.Contains(name)) 
                .ToListAsync();
        }

        public async Task<bool> UserExistsAsync(string email)
        {
            return await _dbContext.Users.AnyAsync(u => u.Email == email);
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

    }
}