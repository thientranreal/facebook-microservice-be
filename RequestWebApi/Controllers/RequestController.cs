using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RequestWebApi.Models;

namespace RequestWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestController : ControllerBase
    {
        private readonly RequestDbContext _dbContext;
        
        public RequestController(RequestDbContext RequestDbContext)
        {
            _dbContext = RequestDbContext;
        }

        // GET: api/Request
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Request>>> GetRequests()
        {
            return await _dbContext.Requests.ToListAsync();
        }

        // GET: api/Request/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Request>> GetRequest(int id)
        {
            var @Request = await _dbContext.Requests.FindAsync(id);

            if (@Request == null)
            {
                return NotFound();
            }

            return @Request;
        }
        
        [HttpPost]
        public async Task<ActionResult> Create(Request Request)
        {
            await _dbContext.Requests.AddAsync(Request);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
        
        
    }
}