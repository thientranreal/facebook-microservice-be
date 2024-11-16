namespace PostWebApi.Repositories;

public class ReactionRepository // nhớ implement IGenericRepository<Reaction>
{
    private readonly PostDbContext _context;

    public ReactionRepository(PostDbContext context)
    {
        _context = context;
    }
    // implement các phương thức của IGenericRepository<Reaction>
    // thêm dô mấy cái phương thức mà ko có sẵn trong IGenericRepository
}