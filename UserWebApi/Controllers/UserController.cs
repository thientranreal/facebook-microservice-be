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
            return await _dbContext.Users
            .Include(u => u.Friends1)
            .Include(u => u.Friends2)
            .ToListAsync();
        }

        // GET: api/user/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _dbContext.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // GET: api/user/search?name=John
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<User>>> SearchUsersByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Không tìm thấy name parameter!");
            }

            var users = await _dbContext.Users
                .Where(u => u.Name.Contains(name)) 
                .ToListAsync();

            if (users == null || users.Count == 0)
            {
                return NotFound("Không tìm thấy user với tên: " + name);
            }

            return Ok(users);
        }
        
        // POST: api/user
        [HttpPost]
        public async Task<ActionResult> Create(User user)
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        // POST: api/user/login
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Email) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest("Email and password are required.");
            }

            var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Email == loginRequest.Email);

            if (user == null || user.Password != loginRequest.Password)
            {
                return Unauthorized(); 
            }

            return Ok(user); 
        }
        // PUT: api/user/5
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(int id, [FromBody] User userUpdate)
        {
            var user = await _dbContext.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound("Could not find user with id: " + id); 
            }

            if (!string.IsNullOrEmpty(userUpdate.Name))
            {
                user.Name = userUpdate.Name;
            }

            if (userUpdate.Birth != default)
            {
                user.Birth = userUpdate.Birth;
            }

            if (!string.IsNullOrEmpty(userUpdate.Avt))
            {
                user.Avt = userUpdate.Avt;
            }

            if (!string.IsNullOrEmpty(userUpdate.Phone))
            {
                user.Phone = userUpdate.Phone;
            }

            if (!string.IsNullOrEmpty(userUpdate.Email))
            {
                user.Email = userUpdate.Email;
            }

            if (!string.IsNullOrEmpty(userUpdate.Gender))
            {
                user.Gender = userUpdate.Gender;
            }

            if (!string.IsNullOrEmpty(userUpdate.Desc))
            {
                user.Desc = userUpdate.Desc;
            }

            if (userUpdate.IsOnline != user.IsOnline) 
            {
                user.IsOnline = userUpdate.IsOnline;
            }

            if (userUpdate.LastActive != default) 
            {
                user.LastActive = userUpdate.LastActive;
            }

            if (!string.IsNullOrEmpty(userUpdate.Address))
            {
                user.Address = userUpdate.Address;
            }

            if (!string.IsNullOrEmpty(userUpdate.Social))
            {
                user.Social = userUpdate.Social;
            }

            if (!string.IsNullOrEmpty(userUpdate.Education))
            {
                user.Education = userUpdate.Education;
            }

            if (!string.IsNullOrEmpty(userUpdate.Relationship))
            {
                user.Relationship = userUpdate.Relationship;
            }

            if (userUpdate.TimeJoin != default) 
            {
                user.TimeJoin = userUpdate.TimeJoin;
            }

            await _dbContext.SaveChangesAsync(); 

            return Ok(user); 
        }
    }
}
