using PostWebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace PostWebApi;

public class PostDbContext : DbContext
{
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Reaction> Reactions { get; set; }
    public DbSet<Story> Stories { get; set; }

    
    public PostDbContext(DbContextOptions<PostDbContext> dbContextOptions) : base(dbContextOptions)
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
        

        // Automatically include Reactions when querying Post
        modelBuilder.Entity<Post>()
            .Navigation(p => p.Reactions)
            .AutoInclude();  // This will automatically load Reactions when loading Post

        // Automatically include Comments when querying Post
        modelBuilder.Entity<Post>()
            .Navigation(p => p.Comments)
            .AutoInclude();  // This will automatically load Comments when loading Post



        // Post has many reactions
        modelBuilder.Entity<Post>()
            .HasMany(p => p.Reactions)
            .WithOne(r => r.Post)
            .HasForeignKey(r => r.PostId)  // Use the correct foreign key
            .IsRequired() // Make foreign key required if needed
            .OnDelete(DeleteBehavior.Cascade);

        // Post has many comments
        modelBuilder.Entity<Post>()
            .HasMany(p => p.Comments)
            .WithOne(c => c.Post)
            .HasForeignKey(c => c.PostId)  // Use the correct foreign key
            .IsRequired() // Make foreign key required if needed
            .OnDelete(DeleteBehavior.Cascade);
        
    }

}