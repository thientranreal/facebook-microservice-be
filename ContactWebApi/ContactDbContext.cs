using ContactWebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace ContactWebApi;

public class ContactDbContext : DbContext
{
    public DbSet<Group> Groups { get; set; }
    public DbSet<Joining> Joinings { get; set; }
    public DbSet<Message> Messages { get; set; }

    public ContactDbContext(DbContextOptions<ContactDbContext> options) : base(options)
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