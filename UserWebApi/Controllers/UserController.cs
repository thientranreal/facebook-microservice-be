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
                return NotFound("Cannot find user with id : " + id);
            }

            return Ok(user);
        }

        // GET: api/user/search?name=John
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<User>>> SearchUsersByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Cannot find name parameter!");
            }

            var users = await _dbContext.Users
                .Where(u => u.Name != null && u.Name.Contains(name)) 
                .ToListAsync();

            if (users == null || users.Count == 0)
            {
                return NotFound("Cannot find user with name: " + name);
            }

            return Ok(users);
        }

        //PUT: api/user/1
        [HttpPut("{userId}")]
        public async Task<ActionResult<User>> UpdateUser (int userId, User user) {
            if (userId <= 0)
            {
                return BadRequest("Invalid userId.");
            }

            var existingUser = await _dbContext.Users.FindAsync(userId);
            if (existingUser == null)
            {
                return NotFound("Cannot find user with id: " + userId);
            }

            // Update các trường chỉ khi có giá trị truyền vào
            if (!string.IsNullOrEmpty(user.Name))
            {
                existingUser.Name = user.Name;
            }

            if (user.Birth != default(DateTime))
            {
                existingUser.Birth = user.Birth;
            }

            if (!string.IsNullOrEmpty(user.Avt))
            {
                existingUser.Avt = user.Avt;
            }

            if (!string.IsNullOrEmpty(user.Phone))
            {
                existingUser.Phone = user.Phone;
            }

            if (!string.IsNullOrEmpty(user.Email))
            {
                existingUser.Email = user.Email;
            }

            if (!string.IsNullOrEmpty(user.Gender))
            {
                existingUser.Gender = user.Gender;
            }

            if (!string.IsNullOrEmpty(user.Desc))
            {
                existingUser.Desc = user.Desc;
            }

            if (user.IsOnline != 0)
            {
                existingUser.IsOnline = user.IsOnline;
            }

            if (user.LastActive != default(DateTime))
            {
                existingUser.LastActive = user.LastActive;
            }

            if (!string.IsNullOrEmpty(user.Password))
            {
                existingUser.Password = user.Password;
            }

            if (!string.IsNullOrEmpty(user.Address))
            {
                existingUser.Address = user.Address;
            }

            if (!string.IsNullOrEmpty(user.Social))
            {
                existingUser.Social = user.Social;
            }

            if (!string.IsNullOrEmpty(user.Education))
            {
                existingUser.Education = user.Education;
            }

            if (!string.IsNullOrEmpty(user.Relationship))
            {
                existingUser.Relationship = user.Relationship;
            }

            if (user.TimeJoin != default(DateTime))
            {
                existingUser.TimeJoin = user.TimeJoin;
            }

            await _dbContext.SaveChangesAsync();

            return Ok(existingUser);
        }

        //PATCH: api/user/upload?userId=1
        [HttpPatch("upload")]
        public async Task<ActionResult<User>> UploadAvatarForUserByUserId(int userId, [FromBody] AvatarUpdateRequest request)
        {
            if (userId <= 0)
            {
                return BadRequest("Invalid userId.");
            }

            var existingUser = await _dbContext.Users.FindAsync(userId);
            if (existingUser == null)
            {
                return NotFound("Cannot find user with id: " + userId);
            }

            if (string.IsNullOrEmpty(request.ImageUrl) || !Uri.IsWellFormedUriString(request.ImageUrl, UriKind.Absolute))
            {
                return BadRequest("Invalid image URL.");
            }

            existingUser.Avt = request.ImageUrl;
            await _dbContext.SaveChangesAsync();  

            return Ok(existingUser);
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
    }
}
