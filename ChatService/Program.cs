using ChatService.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSignalR();

// Add config CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000") // Nguồn gốc frontend
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials(); // Thêm nếu bạn sử dụng cookie hoặc thông tin xác thực
        });
});

var app = builder.Build();

// Use the CORS policy.
app.UseCors("AllowSpecificOrigin");

app.MapHub<ChatHub>("/chathub");

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();