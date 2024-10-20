using Microsoft.EntityFrameworkCore;
using PostWebApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });

// Database Context Dependency Injection
var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
var dbName = Environment.GetEnvironmentVariable("DB_NAME");
var dbPassword = Environment.GetEnvironmentVariable("DB_ROOT_PASSWORD");

var connectionString = $"server={dbHost};port=3306;database={dbName};user=root;password={dbPassword}";
builder.Services.AddDbContext<PostDbContext>(o => o.UseMySQL(connectionString));
//=========================================

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();