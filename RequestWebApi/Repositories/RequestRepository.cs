using RequestWebApi.Models;

namespace RequestWebApi.Repositories
{
    public class RequestRepository : GenericRepository<Request>, IRequestRepository
    {
        public RequestRepository(RequestDbContext context) : base(context)
        {
        }
    }

    public interface IRequestRepository : IGenericRepository<Request>
    {
    }
}