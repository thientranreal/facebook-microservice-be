﻿using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostWebApi.Models;
using PostWebApi.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly PostDbContext _dbContext;
        private readonly GoogleDriveService _googleDriveService;
        
        public PostController(PostDbContext postDbContext,GoogleDriveService googleDriveService)
        {
            _dbContext = postDbContext;
            _googleDriveService = googleDriveService;
        }

        // Helper function for image upload
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
        
        // GET:(đối với khi lấy post hiển thị lên home) http://localhost:8001/api/post/${currentUserId}?lastPostId=${lastPostId}&limit=${postsPerPage}
        //GET: (đối với khi lấy post của một user)  http://localhost:8001/api/post/${currentUserId}/${userId}?lastPostId=${lastPostId}&limit=${postsPerPage}
       [HttpGet("{currentUserId}/{userId?}")]
       public async Task<ActionResult<IEnumerable<Post>>> GetPosts(int currentUserId, int? userId, int? lastPostId, int limit)
       {
           // Ensure lastPostId is set to 0 if null to avoid issues in query comparisons
           lastPostId ??= 0;

           IQueryable<Post> query = _dbContext.Posts
               .OrderByDescending(post => post.timeline); // Order by newest posts first
           
           if (lastPostId != 0)
           {
               query = query.Where(post => post.id < lastPostId);
           }
       
           // Apply user filter if userId is provided
           if (userId.HasValue)
           {
               query = query.Where(post => post.userId == userId.Value);
           }
       
           // Fetch limited posts according to the specified limit
           var posts = await query
               .Take(limit)
               .ToListAsync();
       
           // Return 204 No Content if no posts found
           if (!posts.Any())
           {
               return NoContent();
           }
       
           // Set likedByCurrentUser flag for each post
           foreach (var post in posts)
           {
               post.likedByCurrentUser = post.Reactions?.Any(r => r.UserId == currentUserId) ?? false;
           }
       
           return Ok(posts);
        }


        //POST : api/post
        [HttpPost]
        public async Task<ActionResult<Post>> Create([FromForm] int userId, [FromForm] string contentPost, [FromForm] IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);  
            }

            try
            {
                string imageUrl = await UploadImageAsync(imageFile);

                // Create and save the post
                var post = new Post
                {
                    userId = userId,
                    content = contentPost,
                    image = imageUrl,
                    timeline = DateTime.Now,
                };

                await _dbContext.Posts.AddAsync(post);
                await _dbContext.SaveChangesAsync();

                return Ok(post);  // Return the created post
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        
        // PUT : api/post/id
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] string contentPost, [FromForm] IFormFile? imageFile,[FromForm] int? discriptionActionToImage)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);  
            }
            // 1. Find the existing post by ID
            var existingPost = await _dbContext.Posts.SingleOrDefaultAsync(p => p.id == id);
            if (existingPost == null)
            {
                return NotFound();
            }
            string imageUrl = existingPost.image;
            
            
            // 3. Update post properties 
            existingPost.content = contentPost;
        
            // 4. Handle image if provided
        
            if (discriptionActionToImage == 1|| discriptionActionToImage==2)
            {
                var regex = new Regex(@"id=([a-zA-Z0-9_-]+)");
                var match = regex.Match(existingPost.image ?? string.Empty);
        
                // Delete previous image if it exists
                if (match.Success && match.Groups.Count > 1)
                   _googleDriveService.DeleteImageFile(match.Groups[1].Value);
                imageUrl = (discriptionActionToImage==1) ?   "" : await UploadImageAsync(imageFile); 
                existingPost.image = imageUrl;
            }
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Log exception
                return StatusCode(500, "An error occurred while saving changes. Please try again later.");
            }
        
            return Ok(existingPost); // Return updated post
        }

        
        //DELETE: api/post/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {   
            
            //KIỂM TRA XEM POST CÓ TỒN TẠI KHÔNG?
            var post = await _dbContext.Posts.SingleOrDefaultAsync(p => p.id == id);
            //NẾU POST KHÔNG TỒN TẠI THÌ TRẢ VỀ 404 
            if (post == null)
            {
                return NotFound();  
            }
            var regex = new Regex(@"id=([a-zA-Z0-9_-]+)");
            var match = regex.Match(post.image);

            if (match.Success && match.Groups.Count > 1)
            {
                // Extract the file ID
                // Console.WriteLine(match.Groups[1].Value);
                var isSuccess = _googleDriveService.DeleteImageFile(match.Groups[1].Value);
               
            }
             _dbContext.Posts.Remove(post);
             await _dbContext.SaveChangesAsync();
             return NoContent();
        }

        

        // GET: api/post/search?content=example&limit=10
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Post>>> SearchPostsByContent(string content, int? limit)
        {
            if (string.IsNullOrEmpty(content))
            {
                return BadRequest("Content parameter is required.");
            }

            int resultsLimit = limit ?? 10;

            var posts = await _dbContext.Posts
                .Where(post => post.content.Contains(content))
                .Take(resultsLimit) 
                .ToListAsync();

            if (posts == null || posts.Count == 0)
            {
                return NotFound($"No posts found with content containing: {content}.");
            }

            return Ok(posts); 
        }


        // thêm hàm này để upload avatar bên profile 
        // POST: api/post/uploadImage
        [HttpPost("uploadImage")]
        public async Task<ActionResult<string>> UploadImageForProfile([FromForm] IFormFile? imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return BadRequest("Image file is required.");
            }

            try
            {
                string imageUrl = await UploadImageAsync(imageFile);
                
                return Ok(new { imageUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

}