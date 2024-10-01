using Microsoft.EntityFrameworkCore;
using NotificationWebApi;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Database Context Dependency Injection
// var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
// var dbName = Environment.GetEnvironmentVariable("DB_NAME");
// var dbPassword = Environment.GetEnvironmentVariable("DB_ROOT_PASSWORD");
var dbHost= "localhost";
var dbName= "notificationdb";
var dbPassword = "";
//
//
var connectionString = $"server={dbHost};port=3306;database={dbName};user=root;password={dbPassword}";
builder.Services.AddDbContext<NotifcationDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 21))));
// // =========================================




var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();