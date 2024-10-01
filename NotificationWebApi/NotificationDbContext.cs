using NotificationWebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace NotificationWebApi;

public class NotificationDbContext : DbContext
{
    public DbSet<Notification> Notifications { get; set; }

    public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options)
    {
        try
        {
            var databaseCreator = Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
            if (databaseCreator != null)
            {
                // Create DB if it can't connect
                if (!databaseCreator.CanConnect()) databaseCreator.Create();
                
                // Create DB if no tables exist
                if (!databaseCreator.HasTables()) databaseCreator.CreateTables();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}