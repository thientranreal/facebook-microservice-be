using ChatService.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSignalR();

var allowHost = Environment.GetEnvironmentVariable("ALLOW_HOST");

// Add config CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000",
                    allowHost)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
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