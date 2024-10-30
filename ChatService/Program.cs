using ChatService.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSignalR();


// Configure CORS to allow any origin, method, and header.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

// Use the CORS policy.
app.UseCors("AllowAll");

app.MapHub<ChatHub>("/chathub");

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();