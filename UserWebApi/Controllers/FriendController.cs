using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            return await _dbContext.Friends.ToListAsync();
        }

        // GET: api/friend/user/1
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<User>>> GetFriends(int userId)
        {
            // Lấy tất cả các mối quan hệ bạn bè liên quan đến UserId
            var friendRelationships = await _dbContext.Friends
                .Where(f => (f.UserId1 == userId || f.UserId2 == userId) && f.IsFriend)
                .ToListAsync();

            if (friendRelationships == null || friendRelationships.Count == 0)
            {
                return NotFound("No friends found for this user.");
            }

            // Lấy danh sách Id của bạn bè
            var friendIds = friendRelationships
                .Select(f => f.UserId1 == userId ? f.UserId2 : f.UserId1)
                .ToList();

            // Truy vấn để lấy danh sách bạn bè từ bảng User
            var friends = await _dbContext.Users
                .Where(u => friendIds.Contains(u.Id))
                .ToListAsync();

            return Ok(friends);
        }
    }
}
