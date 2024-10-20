using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostWebApi.Models;

namespace PostWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly PostDbContext _dbContext;
        
        public PostController(PostDbContext postDbContext)
        {
            _dbContext = postDbContext;
        }
        
        // GET: api/post/currentUserId/userId
        [HttpGet("{currentUserId}/{userId?}")]
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts(int currentUserId, int? userId)
        {
            // If userId is provided, filter posts by userId
            if (userId.HasValue)
            {
                var userPosts = await _dbContext.Posts
                    .Where(post => post.userId == userId.Value)
                    .ToListAsync();

                if (!userPosts.Any()) // No posts found for the user
                {
                    return NoContent(); // 204 No Content
                }

                // Set likedByCurrentUser for each post
                foreach (var post in userPosts)
                {
                    post.likedByCurrentUser = post.Reactions?.Any(r => r.UserId == currentUserId) ?? false;
                }

                return Ok(userPosts); // Return filtered posts
            }

            // If no userId is provided, return all posts
            var posts = await _dbContext.Posts.ToListAsync();
    
            if (!posts.Any()) // No posts found
            {
                return NoContent(); // 204 No Content
            }

            // Set likedByCurrentUser for each post
            foreach (var post in posts)
            {
                post.likedByCurrentUser = post.Reactions?.Any(r => r.UserId == currentUserId) ?? false;
            }

            return Ok(posts); // Return all posts if no userId is provided
        }

        

        //POST : api/post
        [HttpPost]
        public async Task<ActionResult<Post>> Create(Post post)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);  
            }
            
            try
            {
                 post.timeline = DateTime.Now;  // Cập nhật lại timeline
                 await _dbContext.Posts.AddAsync(post);
                 await _dbContext.SaveChangesAsync();
                 return Ok(post);  // Trả về kết quả khi tạo thành công
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        //PUT : api/post/id
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Post post)
        {
            // KIỂM TRA DỮ LIỆU ĐẦU VÀO CÓ HỢP LỆ KHÔNG?
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);  // Trả về 400 Bad Request với thông tin lỗi
            }

            // 2. Tìm post hiện tại theo id
            var existingPost = await _dbContext.Posts.SingleOrDefaultAsync(p => p.id == id);

            // 3. Nếu không tồn tại post, trả về NotFound (404)
            if (existingPost == null)
            {
                return NotFound();  // Trả về 404 nếu không tìm thấy post
            }
            
            // 5. Cập nhật các thuộc tính của post hiện tại với giá trị mới
            existingPost.content = post.content;
            existingPost.image = post.image;

            // 6. Lưu thay đổi vào database
            await _dbContext.SaveChangesAsync();

            // 7. Trả về NoContent (204) để chỉ ra rằng cập nhật thành công
            return NoContent();
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
            //NẾU POST TỒN TẠI THÌ THỰC HIỆN XÓA POST
            _dbContext.Posts.Remove(post);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}