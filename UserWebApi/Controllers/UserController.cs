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

        [HttpPost]
        public async Task<ActionResult> Create(User user)
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
        
        [HttpPost("login")]
        public async Task<ActionResult> Login(string email, string password)
        {
            // Tìm người dùng theo email
            var user = await _dbContext.Users
                .SingleOrDefaultAsync(u => u.Email == email);
    
            if (user == null || user.Password != password) // Lưu ý: so sánh mật khẩu trực tiếp không an toàn
            {
                return Unauthorized(); // Trả về 401 Unauthorized nếu không tìm thấy người dùng hoặc mật khẩu không đúng
            }

            return Ok(user); // Trả về thông tin người dùng nếu đăng nhập thành công
        }
    }
}
