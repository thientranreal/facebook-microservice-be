namespace NotificationWebApi;



using Microsoft.EntityFrameworkCore;
public class NotifcationDbContext : DbContext
{
    public NotifcationDbContext(DbContextOptions<NotifcationDbContext> options) : base(options) { }

    public DbSet<Notification.Models.Notification> Notifications { get; set; }
}
