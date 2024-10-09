using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserWebApi.Models;

namespace UserWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserDbContext _dbContext;
        
        public UserController(UserDbContext userDbContext)
        {
            _dbContext = userDbContext;
        }

        // GET: api/user
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _dbContext.Users.ToListAsync();
        }

        // GET: api/user/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var @user = await _dbContext.Users.FindAsync(id);

            if (@user == null)
            {
                return NotFound();
            }

            return @user;
        }
        
        [HttpPost]
        public async Task<ActionResult> Create(User user)
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
        
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Email) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest("Email and password are required.");
            }

            // Tìm người dùng theo email
            var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Email == loginRequest.Email);

            if (user == null || user.Password != loginRequest.Password)
            {
                return Unauthorized(); 
            }
            return Ok(user); 
        }
    }
}
