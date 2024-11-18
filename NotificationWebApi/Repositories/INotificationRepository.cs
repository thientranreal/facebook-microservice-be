using NotificationWebApi.Models;

namespace NotificationWebApi.Repositories;

public interface INotificationRepository : IGenericRepository<Notification>
{
    Task<IEnumerable<Notification>> GetNotificationsByReceiverAsync(int receiverId);
    Task MarkAllAsReadAsync(int receiverId);
    Task<Notification> GetNotificationByDetailsAsync(int user, int receiver, int post, int action_n);
}