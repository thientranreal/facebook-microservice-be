using RequestWebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using RequestWebApi.Models;

namespace RequestWebApi;

public class RequestDbContext : DbContext
{
    public DbSet<Request> Requests { get; set; }

    
    public RequestDbContext(DbContextOptions<RequestDbContext> dbContextOptions) : base(dbContextOptions)
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