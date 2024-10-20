using ChatService.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSignalR();

var app = builder.Build();

app.MapHub<ChatHub>("/chathub");

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();