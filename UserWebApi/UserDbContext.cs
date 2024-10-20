using UserWebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace UserWebApi;

public class UserDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Friend> Friends { get; set; }
    
    public UserDbContext(DbContextOptions<UserDbContext> dbContextOptions) : base(dbContextOptions)
    {
        try
        {
            var databaseCreator = Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
            if (databaseCreator != null)
            {
                if(!databaseCreator.CanConnect()) databaseCreator.Create();
                 
                if(!databaseCreator.HasTables()) databaseCreator.CreateTables();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }
}