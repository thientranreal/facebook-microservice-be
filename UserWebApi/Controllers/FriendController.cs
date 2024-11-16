using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using UserWebApi.Models;
using System.Net;

namespace UserWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendController : ControllerBase
    {
        private readonly UserDbContext _dbContext;
        private readonly IHttpClientFactory _httpClientFactory;

        public FriendController(UserDbContext dbContext, IHttpClientFactory httpClientFactory)
        {
            _dbContext = dbContext;
            _httpClientFactory = httpClientFactory;
        }

        // lấy tất cả mối quan hệ trong bảng friend
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
                // return NotFound("Hiện không có bất kỳ bạn bè nào.");
                return Ok(new List<User>());
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
        
        // DELETE: api/friend/remove/{id}/{idFriend}
        [HttpDelete("remove/{id}/{idFriend}")]
        public async Task<IActionResult> RemoveFriendship(int id, int idFriend)
        {
            // Tìm các quan hệ bạn bè mà UserId1 và UserId2 tương ứng với id và idFriend
            var friendships = await _dbContext.Friends
                .Where(f => 
                    ((f.UserId1 == id && f.UserId2 == idFriend) || (f.UserId1 == idFriend && f.UserId2 == id)) 
                    && f.IsFriend)
                .ToListAsync();

            if (friendships == null || friendships.Count == 0)
            {
                return NotFound("Không tìm thấy quan hệ bạn bè nào với ID được cung cấp.");
            }

            // Xóa các quan hệ bạn bè tìm thấy
            _dbContext.Friends.RemoveRange(friendships);

            // Lưu thay đổi vào cơ sở dữ liệu
            await _dbContext.SaveChangesAsync();

            return Ok("Quan hệ bạn bè đã được xóa thành công.");
        }
        
        
        // GET: api/friend/{userId1}/{userId2}
        [HttpGet("{userId1}/{userId2}")]
        public async Task<ActionResult<Friend>> GetFriendshipBetweenTwoUsers(int userId1, int userId2)
        {
            // Kiểm tra mối quan hệ bạn bè giữa userId1 và userId2
            var friendship = await _dbContext.Friends
                .Include(f => f.User1) // Include để lấy chi tiết User1
                .Include(f => f.User2) // Include để lấy chi tiết User2
                .FirstOrDefaultAsync(f => 
                    (f.UserId1 == userId1 && f.UserId2 == userId2) || 
                    (f.UserId1 == userId2 && f.UserId2 == userId1));

            if (friendship == null)
            {
                return NotFound("Không tìm thấy mối quan hệ bạn bè giữa hai người dùng.");
            }

            return Ok(friendship);
        }
        
        
        // [HttpGet("nonfriends/{userId}")]
        // public async Task<IActionResult> GetNonFriends(int userId)
        // {
        //     var httpClient = _httpClientFactory.CreateClient();
        //     var url = $"http://requestwebapi:8080/api/request/{userId}"; // Gọi API `requestwebapi` với userId đúng
        //
        //     // Gửi yêu cầu GET tới API `requestwebapi`
        //     var httpResponseMessage = await httpClient.GetAsync(url);
        //
        //     // Kiểm tra xem yêu cầu có thành công hay không
        //     if (httpResponseMessage.IsSuccessStatusCode)
        //     {
        //         // Đọc nội dung phản hồi từ HTTP response
        //         var content = await httpResponseMessage.Content.ReadAsStringAsync();
        //
        //         // Deserialize vào danh sách Request (thay vì dynamic)
        //         var friendRequests = JsonConvert.DeserializeObject<List<Request>>(content);
        //
        //         // Trả về danh sách friendRequests dưới dạng HTTP response
        //         return Ok(friendRequests);
        //     }
        //     else
        //     {
        //         // Nếu có lỗi, trả về mã lỗi và thông báo
        //         return StatusCode((int)httpResponseMessage.StatusCode, "Failed to get non-friends data.");
        //     }
        // }


        
        [HttpGet("nonfriends/{userId}")]
        public async Task<IActionResult> GetNonFriends(int userId)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var url = $"http://requestwebapi:8080/api/request/{userId}"; // Gọi API `requestwebapi` với userId đúng

            // Gửi yêu cầu GET tới API `requestwebapi`
            var httpResponseMessage = await httpClient.GetAsync(url);

            // Kiểm tra xem yêu cầu có thành công hay không
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                return StatusCode((int)httpResponseMessage.StatusCode, "Failed to get non-friends data.");
            }

            // Đọc nội dung phản hồi từ HTTP response
            var content = await httpResponseMessage.Content.ReadAsStringAsync();

            // Deserialize vào danh sách Request (thay vì dynamic)
            var requests = JsonConvert.DeserializeObject<List<Request>>(content);

            // Lấy tất cả bạn bè của userId từ database
            var friendRelationships = await _dbContext.Friends
                .Where(f => (f.UserId1 == userId || f.UserId2 == userId) && f.IsFriend)
                .ToListAsync();

            // Lấy danh sách bạn bè của userId
            var friendIds = friendRelationships
                .Select(f => f.UserId1 == userId ? f.UserId2 : f.UserId1)
                .ToList();

            // Lấy danh sách tất cả người dùng
            var allUsers = await _dbContext.Users.ToListAsync();

            // Lọc ra danh sách người dùng không phải bạn và không có yêu cầu kết bạn
            var nonFriends = allUsers
                .Where(u => u.Id != userId && !friendIds.Contains(u.Id) && !requests.Any(r => (r.Sender == u.Id || r.Receiver == u.Id)))
                .ToList();

            // Trả về danh sách người dùng không phải bạn và không có yêu cầu kết bạn
            return Ok(nonFriends);
        }

        
        
        
        
        
        
        
        
        // POST: api/friend/create-and-delete-request
        [HttpPost("create-and-delete-request")]
        public async Task<ActionResult<Friend>> CreateFriendAndDeleteRequest([FromBody] FriendRequestData friendData)
        {
            // Lấy thời gian hệ thống hiện tại theo múi giờ Việt Nam
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime vietnamTimeNow = TimeZoneInfo.ConvertTime(DateTime.Now, vietnamTimeZone);
            if (friendData == null || friendData.UserId1 == friendData.UserId2)
            {
                return BadRequest("Không thể kết bạn với chính mình!");
            }

            // Kiểm tra nếu quan hệ bạn bè đã tồn tại
            var existingFriendship = await _dbContext.Friends
                .FirstOrDefaultAsync(f => 
                    (f.UserId1 == friendData.UserId1 && f.UserId2 == friendData.UserId2) || 
                    (f.UserId1 == friendData.UserId2 && f.UserId2 == friendData.UserId1));

            if (existingFriendship != null)
            {
                return Conflict("Bạn đã là bạn bè!");
            }

            // Bắt đầu transaction của Saga
            var transactionSuccessful = false;
            var errorMessage = string.Empty;

            // Bước 1: Tạo quan hệ bạn bè
            var friend = new Friend
            {
                UserId1 = friendData.UserId1,
                UserId2 = friendData.UserId2,
                IsFriend = friendData.IsFriend,
                TimeLine = vietnamTimeNow
            };

            try
            {
                _dbContext.Friends.Add(friend);
                await _dbContext.SaveChangesAsync();
                transactionSuccessful = true;  // Bước 1 thành công
            }
            catch (Exception ex)
            {
                errorMessage = $"Lỗi khi tạo quan hệ bạn bè: {ex.Message}";
                return StatusCode(500, errorMessage);
            }

            // Bước 2: Xóa yêu cầu kết bạn (Friend Request) thông qua requestId
            if (transactionSuccessful)
            {
                var requestClient = _httpClientFactory.CreateClient();
                var deleteRequestUrl = $"http://requestwebapi:8080/api/request/{friendData.RequestId}"; // Gọi API để xóa yêu cầu kết bạn

                try
                {
                    var deleteResponse = await requestClient.DeleteAsync(deleteRequestUrl);
                    if (deleteResponse.StatusCode == HttpStatusCode.NoContent) // 204 No Content
                    {
                        // Yêu cầu xóa thành công
                        transactionSuccessful = true;
                    }
                    else
                    {
                        // Nếu xóa yêu cầu kết bạn thất bại, thực hiện hành động bồi thường
                        transactionSuccessful = false;
                        errorMessage = "Không thể xóa yêu cầu kết bạn, sẽ xóa quan hệ bạn bè.";
                    }
                }
                catch (Exception ex)
                {
                    transactionSuccessful = false;
                    errorMessage = $"Lỗi khi gọi API xóa yêu cầu kết bạn: {ex.Message}";
                }
            }

            // Nếu có lỗi ở Bước 2, hủy Bước 1
            if (!transactionSuccessful)
            {
                // Bồi thường: Xóa quan hệ bạn bè đã tạo ở bước 1
                var createdFriendship = await _dbContext.Friends
                    .FirstOrDefaultAsync(f => 
                        (f.UserId1 == friendData.UserId1 && f.UserId2 == friendData.UserId2) || 
                        (f.UserId1 == friendData.UserId2 && f.UserId2 == friendData.UserId1));

                if (createdFriendship != null)
                {
                    _dbContext.Friends.Remove(createdFriendship);
                    await _dbContext.SaveChangesAsync();
                }

                return StatusCode(500, errorMessage);
            }

            // Trả về kết quả thành công
            return CreatedAtAction(nameof(GetAllFriends), new { id = friend.Id }, friend);
        }
        
    }
    
}
