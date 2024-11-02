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

        // PUT: api/user/5
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(int id, [FromBody] User userUpdate)
        {
            var user = await _dbContext.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound("Could not find user with id: " + id);
            }

            // Kiểm tra các trường thông tin cá nhân bắt buộc
            if (string.IsNullOrEmpty(userUpdate.Name) ||
                userUpdate.Birth == default ||
                string.IsNullOrEmpty(userUpdate.Email) ||
                string.IsNullOrEmpty(userUpdate.Phone) ||
                string.IsNullOrEmpty(userUpdate.Gender) ||
                string.IsNullOrEmpty(userUpdate.Address))
            {
                return BadRequest("Personal Information fields, such as name, phone, email, gender, address, birth, are required.");
            }

            // Cập nhật dữ liệu nếu có giá trị mới
            user.Name = userUpdate.Name;
            user.Birth = userUpdate.Birth;
            user.Email = userUpdate.Email;
            user.Phone = userUpdate.Phone;
            user.Gender = userUpdate.Gender;
            user.Address = userUpdate.Address;

            // Các trường còn lại có thể có hoặc không
            if (userUpdate.Avt != null)
            {
                user.Avt = userUpdate.Avt;
            }

            if (userUpdate.Desc != null)
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

            if (userUpdate.Social != null)
            {
                user.Social = userUpdate.Social;
            }

            if (userUpdate.Education != null)
            {
                user.Education = userUpdate.Education;
            }

            if (userUpdate.Relationship != null)
            {
                user.Relationship = userUpdate.Relationship;
            }

            if (userUpdate.TimeJoin != default)
            {
                user.TimeJoin = userUpdate.TimeJoin;
            }

            // Cập nhật password nếu không rỗng
            if (!string.IsNullOrEmpty(userUpdate.Password))
            {
                user.Password = userUpdate.Password;
            }

            await _dbContext.SaveChangesAsync();

            return Ok(user);
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
