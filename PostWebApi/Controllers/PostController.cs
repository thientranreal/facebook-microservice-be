using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostWebApi.Models;
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
        
        public PostController(PostDbContext postDbContext)
        {
            _dbContext = postDbContext;
        }

        // GET: api/post
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
        {
            return await _dbContext.Posts.ToListAsync();
        }

        // GET: api/post/user/1
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Post>>> GetPostsByUserId(int userId)
        {
            var posts = await _dbContext.Posts
                .Where(post => post.userId == userId)
                .ToListAsync();

            if (posts == null || posts.Count == 0)
            {
                return NotFound($"No posts found for userId: {userId}.");
            }

            return Ok(posts); 
        }

        // GET: api/post/search?content=example
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Post>>> SearchPostsByContent(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return BadRequest("Content parameter is required.");
            }

            var posts = await _dbContext.Posts
                .Where(post => post.content.Contains(content))
                .ToListAsync();

            if (posts == null || posts.Count == 0)
            {
                return NotFound($"No posts found with content containing: {content}.");
            }

            return Ok(posts); 
        }

        // GET: api/post/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPost(int id)
        {
            var post = await _dbContext.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            return post;
        }

        // POST: api/post
        [HttpPost]
        public async Task<ActionResult> Create(Post post)
        {
            await _dbContext.Posts.AddAsync(post);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
