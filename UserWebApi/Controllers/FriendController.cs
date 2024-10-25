using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserWebApi.Models;

namespace UserWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendController : ControllerBase
    {
        private readonly UserDbContext _dbContext;

        public FriendController(UserDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: api/friend
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Friend>>> GetAllFriends()
        {
            return await _dbContext.Friends
                .Include(f => f.User1)  
                .Include(f => f.User2)
                .ToListAsync();
        }

        // GET: api/friend/user/1
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<User>>> GetFriends(int userId)
        {
            // Lấy tất cả các mối quan hệ bạn bè liên quan đến UserId
            var friendRelationships = await _dbContext.Friends
                .Where(f => (f.UserId1 == userId || f.UserId2 == userId) && f.IsFriend)
                .Include(f => f.User1)
                .Include(f => f.User2)
                .ToListAsync();

            if (friendRelationships == null || friendRelationships.Count == 0)
            {
                return NotFound("Hiện không có bất kỳ bạn bè nào.");
            }

            // Lấy danh sách bạn bè từ mối quan hệ
            var friends = friendRelationships
                .Select(f => f.UserId1 == userId ? f.User2 : f.User1)  // Nếu UserId1 là userId hiện tại, lấy User2 làm bạn bè, ngược lại lấy User1
                .ToList();

            return Ok(friends);
        }

        // POST: api/friend
        [HttpPost]
        public async Task<ActionResult<Friend>> CreateFriendship([FromBody] Friend friend)
        {
            if (friend == null || friend.UserId1 == friend.UserId2)
            {
                return BadRequest("Không thể kết bạn !");
            }

            // Kiểm tra nếu quan hệ bạn bè đã tồn tại
            var existingFriendship = await _dbContext.Friends
                .FirstOrDefaultAsync(f => 
                    (f.UserId1 == friend.UserId1 && f.UserId2 == friend.UserId2) || 
                    (f.UserId1 == friend.UserId2 && f.UserId2 == friend.UserId1));

            if (existingFriendship != null)
            {
                return Conflict("Bạn đã tồn tại !");
            }

            _dbContext.Friends.Add(friend);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAllFriends), new { id = friend.Id }, friend);
        }
    }
}
