
using System.Linq.Expressions;

namespace RequestWebApi.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            int pageNumber = 1, 
            int pageSize = 20);

        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task DeleteAsync(int id);
        Task DeleteAsync(Expression<Func<T, bool>> filter);
        Task<int> CountAsync(Expression<Func<T, bool>>? filter = null);
    }
}

