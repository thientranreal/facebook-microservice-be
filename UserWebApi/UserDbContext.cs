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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User has many friendships as UserId1
        modelBuilder.Entity<Friend>()
            .HasOne(f => f.User1) // Friend liên kết với User1
            .WithMany(u => u.Friends1) // Danh sách bạn bè mà người dùng đã kết bạn
            .HasForeignKey(f => f.UserId1) // Khóa ngoại UserId1
            .OnDelete(DeleteBehavior.Cascade); // Xóa theo chuỗi

        // User has many friendships as UserId2
        modelBuilder.Entity<Friend>()
            .HasOne(f => f.User2) // Friend liên kết với User2
            .WithMany(u => u.Friends2) // Danh sách bạn bè đã kết bạn với người dùng
            .HasForeignKey(f => f.UserId2) // Khóa ngoại UserId2
            .OnDelete(DeleteBehavior.Cascade); // Xóa theo chuỗi
    }
}