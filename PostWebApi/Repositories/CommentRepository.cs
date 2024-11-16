using PostWebApi.Models;

namespace PostWebApi.Repositories;

public class CommentRepository  // nhớ implement IGenericRepository<Comment>
{
    private readonly PostDbContext _context;

    public CommentRepository(PostDbContext context)
    {
        _context = context;
    }
    // implement các phương thức của IGenericRepository<Comment>
    // thêm dô mấy cái phương thức mà ko có sẵn trong IGenericRepository
}