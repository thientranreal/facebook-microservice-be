using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();
builder.Services.AddOcelot(builder.Configuration);


// Add config CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000", 
                "http://192.168.176.1:3000")  // Đảm bảo thêm tất cả các URL có thể sử dụng
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();  // Cho phép cookies được gửi
    });
});

var app = builder.Build();

// Use middleware CORS
app.UseCors("AllowAll");

await app.UseOcelot();
app.Run();