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
            var twentyFourHoursAgo = DateTime.Now.AddHours(-24);

            // Lọc story có timeline trong vòng 24 giờ và sắp xếp theo timeline giảm dần
            var stories = await _dbContext.Stories
                .Where(s => s.timeline >= twentyFourHoursAgo) // Chỉ lấy những story có timeline trong 24 giờ qua
                .OrderByDescending(s => s.timeline) // Sắp xếp từ mới đến cũ
                .ToListAsync();

            if (stories == null || !stories.Any())
            {
                return NotFound();
            }

            return Ok(stories);
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