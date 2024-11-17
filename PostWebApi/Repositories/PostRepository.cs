using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostWebApi.Models;

namespace PostWebApi.Repositories;

public class PostRepository : IGenericRepository<Post>// nhớ implement IGenericRepository<Post>
{
    private readonly PostDbContext _dbContext;

    public PostRepository(PostDbContext context)
    {
        _dbContext = context;
    }
    // implement các phương thức của IGenericRepository<Post>
    // thêm dô mấy cái phương thức mà ko có sẵn trong IGenericRepository

    public async Task<IEnumerable<Post>> GetAllAsync()
    {
        throw new NotImplementedException();
    }
    
    public async Task<Post?> GetByIdAsync(int id)
    {
        var post = await _dbContext.Posts
            .Include(p => p.Comments)
            .Include(p => p.Reactions)
            .FirstOrDefaultAsync(p => p.id == id);
        return post;
    }

    public async Task AddAsync(Post entity)
    {
        await _dbContext.Posts.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Post entity)
    {
        _dbContext.Posts.Update(entity);
    }

    public Task DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(Post entity)
    {
        
        _dbContext.Posts.Remove(entity);
        await SaveChangesAsync();
       
    }
    
    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
    
    
    public async Task<Post?> Exited(int id)
    {
        return await _dbContext.Posts.SingleOrDefaultAsync(p => p.id == id);
    }
    
    //method GetPosts
    public async Task<IEnumerable<Post>> GetPosts(int limit,int? lastPostId, int? userId)
    {
        IQueryable<Post> query = _dbContext.Posts
            .OrderByDescending(post => post.timeline); // Order by newest posts first

        if (lastPostId != 0)
        {
            query = query.Where(post => post.id < lastPostId);
        }

        // Apply user filter if userId is provided
        if (userId.HasValue)
        {
            query = query.Where(post => post.userId == userId.Value);
        }

        // Fetch limited posts according to the specified limit
        return await query
            .Take(limit)
            .ToListAsync();
    }

    //method GetPostSearch
    public async Task<IEnumerable<Post>> SearchPostsByContent(string content,int resultsLimit,int resultsOffset)
    {
        return await _dbContext.Posts
            .Where(post => post.content.Contains(content))
            .Skip(resultsOffset) // Bỏ qua số lượng bản ghi dựa trên offset
            .Take(resultsLimit)  // Lấy số lượng bản ghi dựa trên limit
            .ToListAsync();
    }
}