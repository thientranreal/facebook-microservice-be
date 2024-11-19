using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostWebApi.Models;
using PostWebApi.Repositories;
using PostWebApi.Services;

namespace PostWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoryController : ControllerBase
    {
        private readonly StoryRepository _storyRepository;
        private readonly GoogleDriveService _googleDriveService;

        
        public StoryController(StoryRepository storyRepository, GoogleDriveService googleDriveService)
        {
            _storyRepository = storyRepository;
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
        [HttpPost("create")]
        public async Task<IActionResult> PostStory([FromForm] IFormFile image, [FromForm] int userId)
        {
            if (image == null || image.Length == 0)
                return BadRequest("Image is required.");

            var imageUrl = await UploadImageAsync(image);
            if (string.IsNullOrEmpty(imageUrl))
                return StatusCode(500, "Failed to upload image to Google Drive.");

            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var story = new Story
            {
                userId = userId,
                image = imageUrl,
                timeline = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone),
            };

            await _storyRepository.AddAsync(story);
            await _storyRepository.SaveChangesAsync();
            return Ok(story);
        }

        [HttpGet]
        public async Task<IActionResult> GetStories()
        {
            var stories = await _storyRepository.GetStoriesWithin24HoursAsync();
            if (stories == null || !stories.Any())
                return NotFound();

            return Ok(stories);
        }        

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetStoriesByUserId(int userId)
        {
            var stories = await _storyRepository.GetStoriesByUserIdAsync(userId);
            if (stories == null || !stories.Any())
                return NotFound();

            return Ok(stories);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Story story)
        {
            await _storyRepository.AddAsync(story);
            await _storyRepository.SaveChangesAsync();
            return Ok();
        }
    }
}