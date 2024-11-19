using UserWebApi.Models;

namespace UserWebApi.Repositories
{
    public interface IUserRepository:IGenericRepository<User>
    {
        Task<IEnumerable<User>> GetUsersAsync();
        //Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByEmailAsync(string email); 
        Task<IEnumerable<User>> SearchUsersByNameAsync(string name); 
        Task<List<User>> SearchUsersByNamePaginationAsync(string name, int limit, int offset);
        Task<bool> UserExistsAsync(string email);
        Task SaveChangesAsync();
    }

}