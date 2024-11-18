using Microsoft.EntityFrameworkCore;

namespace UserWebApi.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly UserDbContext _user;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(UserDbContext user)
    {
        _user = user;
        _dbSet = _user.Set<T>();
    }

    public async Task<IEnumerable<T>?> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _user.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _user.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _user.SaveChangesAsync();
        }
    }
}