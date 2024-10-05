using Microsoft.EntityFrameworkCore;
using PostWebApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();


// Database Context Dependency Injection
var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
var dbName = Environment.GetEnvironmentVariable("DB_NAME");
var dbPassword = Environment.GetEnvironmentVariable("DB_ROOT_PASSWORD");

var connectionString = $"server={dbHost};port=3306;database={dbName};user=root;password={dbPassword}";
builder.Services.AddDbContext<PostDbContext>(o => o.UseMySQL(connectionString));
//=========================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseCors("AllowAll");
// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();