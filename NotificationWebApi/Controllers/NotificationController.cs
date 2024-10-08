using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotificationWebApi.Models;
using NotificationWebApi.Repositories;

namespace NotificationWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationDbContext _context;

        public NotificationController(NotificationDbContext context)
        {
            _context = context;
        }

        // GET: api/Notification
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Notification>>> GetNotifications()
        {
            return await _context.Notifications.ToListAsync();
        }

        // GET: api/Notification/receiver/5
        [HttpGet("receiver/{receiverId}")]
        public async Task<ActionResult<IEnumerable<Notification>>> GetNotificationByReceiver(int receiverId)
        {
            var notifications = await _context.Notifications.Where(n => n.receiver == receiverId).ToListAsync();

            if (notifications == null || !notifications.Any())
            {
                return NotFound();
            }

            return Ok(notifications);
        }


        [HttpGet("{id}")]
                public async Task<ActionResult<Notification>> GetNotification(int id)
                {
                    var notification = await _context.Notifications.FindAsync(id);
        
                    if (notification == null)
                    {
                        return NotFound();
                    }
        
                    return notification;
                }

        // PUT: api/Notification/5
        [HttpPut("{id}")]
        public IActionResult MarkAsRead(int id)
                {
                    var notification = _context.Notifications.FirstOrDefault(n => n.id == id);
        
                    if (notification == null)
                    {
                        return NotFound(new { message = "Notification not found." });
                    }
        
                    notification.is_read = 1;
        
                    _context.SaveChanges();
        
                    return Ok(new { message = "Notification marked as read." });
                }

        // POST: api/Notification
        [HttpPost]
        public async Task<ActionResult<Notification>> PostNotification(Notification notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetNotification", new { id = notification.id }, notification);
        }

        // DELETE: api/Notification/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
            {
                return NotFound();
            }

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool NotificationExists(int id)
        {
            return _context.Notifications.Any(e => e.id == id);
        }
         [HttpPut("markAllAsRead/{receiverId}")]
        public IActionResult MarkAllAsRead(int receiverId)
        {
            var notifications = _context.Notifications
                .Where(n => n.receiver == receiverId) 
                .ToList();

            if (notifications.Count == 0)
            {
                return NotFound(new { message = "No notifications found for this receiver." });
            }

            foreach (var notification in notifications)
            {
                notification.is_read = 1; 
            }

            _context.SaveChanges(); 

            return Ok(new { message = "All notifications marked as read for the specified receiver." });
        }
    }
}
