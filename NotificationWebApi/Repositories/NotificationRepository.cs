using Microsoft.EntityFrameworkCore;
using NotificationWebApi.Models;

namespace NotificationWebApi.Repositories;

public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
{
    public NotificationRepository(NotificationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Notification>> GetNotificationsByReceiverAsync(int receiverId)
    {
        return await _dbSet
            .Where(n => n.receiver == receiverId)
            .OrderByDescending(n => n.timeline)
            .ToListAsync();
    }

    public async Task MarkAllAsReadAsync(int receiverId)
    {
        var notifications = _dbSet.Where(n => n.receiver == receiverId).ToList();
        notifications.ForEach(n => n.is_read = 1);
        await _context.SaveChangesAsync();
    }

    public async Task<Notification> GetNotificationByDetailsAsync(int user, int receiver, int post, int action_n)
    {
        return await _dbSet
            .FirstOrDefaultAsync(n => n.user == user && n.receiver == receiver && n.post == post && n.action_n == action_n);
    }
}