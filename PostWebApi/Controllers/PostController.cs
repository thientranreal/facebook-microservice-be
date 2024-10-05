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

        // GET: api/Post
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
        {
            return await _dbContext.Posts.ToListAsync();
        }

        // GET: api/Post/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPost(int id)
        {
            var @Post = await _dbContext.Posts.FindAsync(id);

            if (@Post == null)
            {
                return NotFound();
            }

            return @Post;
        }
        
        [HttpPost]
        public async Task<ActionResult> Create(Post post)
        {
            await _dbContext.Posts.AddAsync(post);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
        
        
    }
}