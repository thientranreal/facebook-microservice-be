using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using RequestWebApi.Models;
using RequestWebApi.Repositories;

namespace RequestWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestController : ControllerBase
    {
        private readonly IRequestRepository _requestRepository;
        private readonly RequestDbContext _context;

        public RequestController(RequestDbContext context, IRequestRepository requestRepository)
        {
            _context = context;
            _requestRepository = requestRepository;
        }

        [HttpGet("requests")]
        public async Task<ActionResult<IEnumerable<Request>>> GetRequests(int? id, int pageNumber = 1, int pageSize = 20)
        {
            // Define filter as a default empty expression
            Expression<Func<Request, bool>>? filter = null;

            // Set the filter condition based on the 'id' parameter
            if (id.HasValue)
            {
                filter = r => r.Receiver == id.Value;
            }

            // Fetch requests with the defined filter
            var requests = await _requestRepository.GetAllAsync(filter, pageNumber, pageSize);

            // Get the total count based on the filter
            var totalRequests = await _requestRepository.CountAsync(filter);
            Response.Headers.Add("X-Total-Count", totalRequests.ToString());

            // If no requests are found, return No Content
            if (!requests.Any())
            {
                return NoContent();
            }

            return Ok(requests);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Request>>> GetRequestsByUserId(int id)
        {
            var requests = await _requestRepository.GetAllAsync(r => r.Sender == id || r.Receiver == id);
            return Ok(requests);
        }

        [HttpPost]
        public async Task<ActionResult> Create(Request request)
        {

            var existingRequest = await _context.Requests
                .FirstOrDefaultAsync(r => 
                    (r.Sender == request.Sender && r.Receiver == request.Receiver) || 
                    (r.Sender == request.Receiver && r.Receiver == request.Sender));

            if (existingRequest != null)
            {
                // Trả về trạng thái tồn tại
                return Conflict(new { status = "exists", message = "Request already exists" });
            }
            await _requestRepository.AddAsync(request);
            return Ok(new { id = request.Id });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequest(int id)
        {
            await _requestRepository.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("{curUserId}/{userId}")]
        public async Task<ActionResult<IEnumerable<Request>>> GetRequestsByCurrentUserIdAndUser(int curUserId, int userId)
        {
            var requests = await _requestRepository.GetAllAsync(r => r.Sender == userId && r.Receiver == curUserId);
            return Ok(requests);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteRequestBySenderAndReceiver(int senderId, int receiverId)
        {
            await _requestRepository.DeleteAsync(r => r.Sender == senderId && r.Receiver == receiverId);
            return NoContent();
        }
    }
}
