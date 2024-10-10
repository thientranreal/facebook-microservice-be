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

// Use middleware CORS
app.UseCors("AllowSpecificOrigin");

await app.UseOcelot();
app.Run();