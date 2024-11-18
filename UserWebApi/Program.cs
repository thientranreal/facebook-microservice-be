using Microsoft.EntityFrameworkCore;
using UserWebApi;
using UserWebApi.Repositories;
using UserWebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add EmailService
builder.Services.AddScoped<IEmailService, EmailService>();
//Add UserRepository
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Database Context Dependency Injection
var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
var dbName = Environment.GetEnvironmentVariable("DB_NAME");
var dbPassword = Environment.GetEnvironmentVariable("DB_ROOT_PASSWORD");

var connectionString = $"server={dbHost};port=3306;database={dbName};user=root;password={dbPassword}";
builder.Services.AddDbContext<UserDbContext>(o => o.UseMySQL(connectionString));

// =========================================
// Thêm dịch vụ Session
builder.Services.AddDistributedMemoryCache(); // Cần cho session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(14); // Thời gian session tồn tại 2 tuần
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpClient();

// =========================================

// Add Session and Distributed Cache
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Use CORS and Session Middleware
app.UseCors("AllowReactApp");

app.UseSession();

app.UseAuthorization();

app.MapControllers();

app.Run();