using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostWebApi.Models;

namespace PostWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoryController : ControllerBase
    {
        private readonly PostDbContext _dbContext;
        
        public StoryController(PostDbContext postDbContext)
        {
            _dbContext = postDbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Story>>> GetStories()
        {
            return await _dbContext.Stories.ToListAsync();
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<Story>>> GetStoriesByUserId(int userId)
        {
            // Lấy danh sách story theo userId
            var stories = await _dbContext.Stories
                .Where(s => s.userId == userId)
                .ToListAsync();

            if (stories == null || !stories.Any())
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy story
            }

            return Ok(stories); // Trả về danh sách story
        }
        
        [HttpPost]
        public async Task<ActionResult> Create(Story story)
        {
            await _dbContext.Stories.AddAsync(story);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
        
        
    }
}