using Microsoft.EntityFrameworkCore;
using NotificationWebApi.Models;

namespace NotificationWebApi.Repositories;

public class NotificationRepo
{
    private readonly NotificationDbContext _context;

    public NotificationRepo(NotificationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Notification>> GetNotificationsByReceiverAsync(int receiver)
    {
        return await _context.Notifications.Where(n => n.receiver == receiver).ToListAsync();
    }
}