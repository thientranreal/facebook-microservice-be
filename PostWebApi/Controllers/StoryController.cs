using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostWebApi.Models;
using PostWebApi.Services;

namespace PostWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoryController : ControllerBase
    {
        private readonly PostDbContext _dbContext;
        private readonly GoogleDriveService _googleDriveService;

        
        public StoryController(PostDbContext postDbContext, GoogleDriveService googleDriveService)
        {
            _dbContext = postDbContext;
            _googleDriveService = googleDriveService;
        }
        
        private async Task<string> UploadImageAsync(IFormFile? imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                using var stream = imageFile.OpenReadStream();
                var mimeType = imageFile.ContentType; // e.g., "image/jpeg", "image/png"
                return _googleDriveService.UploadImage(stream, imageFile.FileName, mimeType);
            }
            return string.Empty;
        }
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> PostStory([FromForm] IFormFile image, [FromForm] int userId)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest("Image is required.");
            }

            // Upload ảnh lên Google Drive và nhận về file ID hoặc URL
            var imageUrl = await UploadImageAsync(image);
    
            if (string.IsNullOrEmpty(imageUrl))
            {
                return StatusCode(500, "Failed to upload image to Google Drive.");
            }
            // Lấy thời gian hiện tại theo UTC
            DateTime timelineUtc = DateTime.UtcNow;

            // Chuyển đổi thời gian UTC sang giờ Việt Nam (UTC+7)
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime timelineVietnam = TimeZoneInfo.ConvertTimeFromUtc(timelineUtc, vietnamTimeZone);

            // Lưu thông tin story vào database
            var story = new Story
            {
                userId = userId,
                image = imageUrl,
                timeline = timelineVietnam,
            };

            _dbContext.Stories.Add(story);
            await _dbContext.SaveChangesAsync();

            return Ok(story); // Trả về story vừa lưu, có thể bao gồm cả URL ảnh
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