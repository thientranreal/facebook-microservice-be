using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NotificationWebApi.Models;
using NotificationWebApi.Repositories;

namespace NotificationWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationController(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Notification>>> GetNotifications()
        {
            return Ok(await _notificationRepository.GetAllAsync());
        }

        [HttpGet("receiver/{receiverId}")]
        public async Task<ActionResult<IEnumerable<Notification>>> GetNotificationByReceiver(int receiverId)
        {
            var notifications = await _notificationRepository.GetNotificationsByReceiverAsync(receiverId);
            return notifications == null ? NotFound() : Ok(notifications);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Notification>> GetNotification(int id)
        {
            var notification = await _notificationRepository.GetByIdAsync(id);
            return notification == null ? NotFound() : Ok(notification);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notification = await _notificationRepository.GetByIdAsync(id);
            if (notification == null) return NotFound(new { message = "Notification not found." });
            
            notification.is_read = 1;
            await _notificationRepository.UpdateAsync(notification);
            
            return Ok(new { message = "Notification marked as read." });
        }

        [HttpPost]
        public async Task<ActionResult<Notification>> PostNotification(Notification notification)
        {
            await _notificationRepository.AddAsync(notification);
            return CreatedAtAction(nameof(GetNotification), new { id = notification.id }, notification);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var notification = await _notificationRepository.GetByIdAsync(id);
            if (notification == null) return NotFound();
            
            await _notificationRepository.DeleteAsync(notification);
            return NoContent();
        }

        [HttpPut("markAllAsRead/{receiverId}")]
        public async Task<IActionResult> MarkAllAsRead(int receiverId)
        {
            await _notificationRepository.MarkAllAsReadAsync(receiverId);
            return Ok(new { message = "All notifications marked as read for the specified receiver." });
        }

        [HttpDelete("delete/{user}/{receiver}/{post}/{action_n}")]
        public async Task<IActionResult> DeleteNotification_2(int user, int receiver, int post, int action_n)
        {
            var notification = await _notificationRepository.GetNotificationByDetailsAsync(user, receiver, post, action_n);
            if (notification == null) return NotFound(new { message = "Notification not found." });
            
            await _notificationRepository.DeleteAsync(notification);
            return Ok(notification);
        }
    }
}
