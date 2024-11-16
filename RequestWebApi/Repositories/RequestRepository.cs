namespace RequestWebApi.Repositories;

public class RequestRepository // nhớ implement IGenericRepository<Request>
{
    private readonly RequestDbContext _context;

    public RequestRepository(RequestDbContext context)
    {
        _context = context;
    }
    // implement các phương thức của IGenericRepository<Request>
    // thêm dô mấy cái phương thức mà ko có sẵn trong IGenericRepository
}