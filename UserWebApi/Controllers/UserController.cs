using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserWebApi.Models;
using UserWebApi.Repositories;
using UserWebApi.Services;

namespace UserWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly UserDbContext _dbContext;
        private readonly IEmailService _emailService;
        private static Dictionary<string, int> _loginAttempts = new(); // Đếm số lần đăng nhập cho mỗi email
        private readonly PasswordHasher<User> _passwordHasher= new PasswordHasher<User>();
        public UserController(IUserRepository userRepository, IEmailService emailService)
        {
            _userRepository = userRepository;
            _emailService = emailService;
        }

        // GET: api/user
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _userRepository.GetUsersAsync();
            return Ok(users);
        }

        // GET: api/user/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);

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
                return BadRequest("Name parameter is required.");
            }
           
            var users = await _userRepository.SearchUsersByNameAsync(name);
            if (users == null || !users.Any())
            {
                return NotFound("Không tìm thấy user với tên: " + name);
            }
            return Ok(users);
        }

        //-----------------------------REGISTER-------------------------
        [HttpPost]
        public async Task<ActionResult> Create(User user)
        {
            if (await _userRepository.UserExistsAsync(user.Email))
                return BadRequest("Email already exists.");

            if (string.IsNullOrEmpty(user.Password))
                return BadRequest("Password is required.");

            // Mã hóa mật khẩu
            if (string.IsNullOrEmpty(user.Password))
            {
                return BadRequest("Password is required.");
            }

            
            user.Password = _passwordHasher.HashPassword(user, user.Password);
            await _userRepository.AddUserAsync(user);
            await _userRepository.SaveChangesAsync();

            // Gửi email xác nhận
            await _emailService.SendConfirmationEmailAsync(user.Email, user.Name);

            return Ok("Confirmation email has been sent.");
        }
// ------------------- CONFIRM EMAIL -------------------
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string email)
        {
            // Tìm người dùng dựa trên email
            var user = await _userRepository.GetUserByEmailAsync(email);

            if (user == null)
            {
                return BadRequest("Email not found.");
            }

            // Kiểm tra nếu người dùng đã xác thực trước đó
            // Nếu TimeJoin là mặc định (ngày 1/1/0001) hoặc là 2010-10-10, thì cập nhật
            if (user.TimeJoin == default(DateTime) || user.TimeJoin == new DateTime(2010, 10, 10, 0, 0, 0))
            {
                // Cập nhật trường `TimeJoin` với thời gian hiện tại
                user.TimeJoin = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(7)).DateTime; // Gán thời gian hiện tại trực tiếp
                await _userRepository.UpdateUserAsync(user);
                await _userRepository.SaveChangesAsync();

                return Ok("Email confirmed successfully.");
            }

            return BadRequest("Email has already been confirmed.");
        }

        //-------------------------------LOGIN-------------------------------------
        
       [HttpPost("login")]
public async Task<ActionResult> Login([FromBody] LoginRequest loginRequest)
{
    // Kiểm tra nếu loginRequest là null
    if (loginRequest == null)
    {
        return BadRequest("Invalid request data.");
    }

    // Kiểm tra nếu Email và Password bị null hoặc rỗng
    if (string.IsNullOrEmpty(loginRequest.Email) || string.IsNullOrEmpty(loginRequest.Password))
    {
        return BadRequest("Email and password are required.");
    }

    // Khởi tạo số lần đăng nhập sai nếu chưa có
    if (!_loginAttempts.ContainsKey(loginRequest.Email))
    {
        _loginAttempts[loginRequest.Email] = 0;
    }

    try
    {
        // Kiểm tra xem người dùng có tồn tại không
        var user = await _userRepository.GetUserByEmailAsync(loginRequest.Email);
        
        // Kiểm tra nếu user bị null
        if (user == null)
        {
            _loginAttempts[loginRequest.Email]++;
            return Unauthorized("Invalid email or password.");
        }
        // Kiểm tra xác thực email (TimeJoin)
        if (user.TimeJoin == new DateTime(2010, 10, 10, 0, 0, 0, DateTimeKind.Utc))
        {
            return BadRequest("Email not verified.");
        }

        // Kiểm tra nếu Password của user bị null
        if (string.IsNullOrEmpty(user.Password))
        {
            return Unauthorized("Invalid email or password.");
        }

        // Kiểm tra mật khẩu đã mã hóa
        var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, loginRequest.Password);
        if (passwordVerificationResult == PasswordVerificationResult.Failed)
        {
            _loginAttempts[loginRequest.Email]++;
            return Unauthorized("Invalid email or password.");
        }

        // Reset số lần đăng nhập khi thành công
        _loginAttempts[loginRequest.Email] = 0;
        
        user.IsOnline = 1;
        await _userRepository.SaveChangesAsync();
        // Lưu userId vào session
        HttpContext.Session.SetString("UserId", user.Id.ToString());
        
        return Ok(new { message = "Login successful", userId = user.Id });
    }
    catch (Exception ex)
    {
        // Ghi log lỗi chi tiết để giúp debug
        Console.WriteLine($"Error: {ex.Message}");
        return StatusCode(500, "Internal server error.");
    }
}

        //------------------------------- LOG OUT -----------------------
[HttpPost("logout")]
public async Task<IActionResult> Logout()
{
    // Lấy userId từ session
    var userId = HttpContext.Session.GetString("UserId");
    if (string.IsNullOrEmpty(userId))
    {
        return Unauthorized("User is not logged in.");
    }

    // Tìm user trong cơ sở dữ liệu
    var user = await _userRepository.GetUserByIdAsync(int.Parse(userId));
    if (user == null)
    {
        return NotFound("User not found.");
    }

    // Cập nhật trạng thái người dùng
    user.IsOnline = 0;
    user.LastActive = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(7)).DateTime;

    await _userRepository.SaveChangesAsync();

    // Xóa session
    HttpContext.Session.Clear();

    return Ok("Logged out successfully.");
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

        // ------------------FORGET PASSWORD---------------        
        [HttpPost("forgetpassword")]
        public async Task<ActionResult> ForgetPassword([FromBody] ForgetPasswordRequest forgetPasswordRequest)
        {
            if (string.IsNullOrEmpty(forgetPasswordRequest.Email))
            {
                return BadRequest("Email is required.");
            }
            
            // Tìm người dùng theo email
            var user = await _userRepository.GetUserByEmailAsync(forgetPasswordRequest.Email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            DateOnly dataUserBirthDate = DateOnly.FromDateTime(user.Birth);
            if (forgetPasswordRequest.BirthDate != dataUserBirthDate)
            {
                return BadRequest("Wrong date of birth!");
            }
            //Cần thay mật khẩu trong database thành 1 chuỗi ngẩu nhiên ở đây ->gửi
            Random rand = new Random();
            string password = "";

            // Tạo mật khẩu gồm 8 số ngẫu nhiên
            for (int i = 0; i < 8; i++)
            {
                password += rand.Next(0, 10); // Chọn số từ 0 đến 9
            }
            
            user.Password = password;
            
            // Gửi mật khẩu hiện tại của người dùng qua email
            var emailBody = $"Your current password is: {user.Password} \n Please change your password once you have successfully logged in!";
            await _emailService.SendEmailAsync(user.Email, "Your Password", emailBody);
            
            //Mã hóa rồi lưu vào database
            user.Password = _passwordHasher.HashPassword(user, user.Password);
            await _userRepository.SaveChangesAsync();
            
            return Ok("Password has been sent to your email.");
        }

        // -----------------------DELETE USER --------------------------------------------
        // Phương thức xóa người dùng theo email
        [HttpDelete("delete-user")]
        public async Task<IActionResult> DeleteUser(string email)
        {
            // Tìm người dùng dựa trên email
            var user = await _userRepository.GetUserByEmailAsync(email);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Xóa người dùng
            await _userRepository.DeleteUserAsync(user);
            await _userRepository.SaveChangesAsync();

            return Ok("User deleted successfully.");
        }
        
        // PUT: api/user/5
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(int id, [FromBody] User userUpdate)
        {
            var user = await _userRepository.GetUserByIdAsync(id);

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

            await _userRepository.SaveChangesAsync(); 

            return Ok(user); 
        }
    }
         //------------------------------------- LOG OUT ----------------------------
 
}
