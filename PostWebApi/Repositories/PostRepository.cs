namespace PostWebApi.Repositories;

public class PostRepository // nhớ implement IGenericRepository<Post>
{
    private readonly PostDbContext _context;

    public PostRepository(PostDbContext context)
    {
        _context = context;
    }
    // implement các phương thức của IGenericRepository<Post>
    // thêm dô mấy cái phương thức mà ko có sẵn trong IGenericRepository
}