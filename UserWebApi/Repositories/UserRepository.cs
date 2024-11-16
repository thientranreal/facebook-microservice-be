using UserWebApi.Models;

namespace UserWebApi.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    private readonly UserDbContext _context;

    public UserRepository(UserDbContext context) : base(context)
    {
        _context = context;
    }
}