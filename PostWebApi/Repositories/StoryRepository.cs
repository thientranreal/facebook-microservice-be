using Microsoft.EntityFrameworkCore;
using PostWebApi.Models;

namespace PostWebApi.Repositories;

public class StoryRepository : IGenericRepository<Story>
{
    private readonly PostDbContext _context;

    public StoryRepository(PostDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Story>> GetAllAsync()
    {
        return await _context.Stories.ToListAsync();
    }

    public async Task<Story?> GetByIdAsync(int id)
    {
        return await _context.Stories.FindAsync(id);
    }

    public async Task AddAsync(Story entity)
    {
        await _context.Stories.AddAsync(entity);
    }

    public async Task UpdateAsync(Story entity)
    {
        _context.Stories.Update(entity);
        await SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var story = await GetByIdAsync(id);
        if (story != null)
        {
            _context.Stories.Remove(story);
            await SaveChangesAsync();
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    // Custom methods specific to StoryRepository
    public async Task<IEnumerable<Story>> GetStoriesWithin24HoursAsync(int user)
    {
        var twentyFourHoursAgo = DateTime.Now.AddHours(-24);
        return await _context.Stories
            .Where(s => s.timeline >= twentyFourHoursAgo && s.userId == user)
            .OrderByDescending(s => s.timeline)
            .ToListAsync();
    }

    public async Task<IEnumerable<Story>> GetStoriesByUserIdAsync(int userId)
    {
        return await _context.Stories
            .Where(s => s.userId == userId)
            .ToListAsync();
    }
}
