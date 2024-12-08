using ChatService.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSignalR();

var allowHost = Environment.GetEnvironmentVariable("ALLOW_HOST");

// Add config CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000", 
                "http://192.168.28.61:3000",
                "http://192.168.176.1:3000")  // Đảm bảo thêm tất cả các URL có thể sử dụng
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();  // Cho phép cookies được gửi
    });

});

var app = builder.Build();

// Use the CORS policy.
app.UseCors("AllowAll");

app.MapHub<ChatHub>("/chathub");

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();