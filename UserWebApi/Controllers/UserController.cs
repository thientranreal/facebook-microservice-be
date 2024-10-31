using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserWebApi.Models;
using UserWebApi.Services;

namespace UserWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserDbContext _dbContext;
        private readonly IEmailService _emailService;
        public UserController(UserDbContext userDbContext, IEmailService emailService)
        {
            _dbContext = userDbContext;
            _emailService = emailService;
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
        
        [HttpPost]
        public async Task<ActionResult> Create(User user)
        {
            if (string.IsNullOrEmpty(user.Email))
            {
                return BadRequest("Email is required.");
            }

            // Kiểm tra xem email đã tồn tại chưa
            var existingUser = await _dbContext.Users
                .SingleOrDefaultAsync(u => u.Email == user.Email);

            if (existingUser != null)
            {
                return BadRequest("Email already exists.");
            }

            // Thêm người dùng mới vào cơ sở dữ liệu
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            // Gửi email xác nhận
            await _emailService.SendConfirmationEmailAsync(user.Email, user.Name);

            return Ok("Confirmation email has been sent.");
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
            // Lưu userId vào Session
            HttpContext.Session.SetString("UserId", user.Id.ToString());

            return Ok(new { message = "Login successful", userId = user.Id });
        }
        [HttpGet("sessionInfo")]
        public IActionResult GetSessionInfo()
        {
            var userId = HttpContext.Session.GetString("UserId");
    
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not logged in");
            }

            return Ok(new { userId });
        }
        
        [HttpPost("forgetpassword")]
        public async Task<ActionResult> ForgetPassword([FromBody] ForgetPasswordRequest forgetPasswordRequest)
        {
            
            if (string.IsNullOrEmpty(forgetPasswordRequest.Email))
            {
                return BadRequest("Email is required.");
            }

            // Tìm người dùng theo email
            var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Email == forgetPasswordRequest.Email);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Console.WriteLine("heheheasdfdfkfkfkfkldcaigidodaidai===jdk=========================================================");
            
            // Gửi mật khẩu hiện tại của người dùng qua email
            var emailBody = $"Your current password is: {user.Password}";
            await _emailService.SendEmailAsync(user.Email, "Your Password", emailBody);

            return Ok("Password has been sent to your email.");
        }
    }
}
