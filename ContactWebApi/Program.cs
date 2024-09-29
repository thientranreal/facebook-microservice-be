using ContactWebApi;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Database Context Dependency Injection
var dbHost = "localhost";
var dbName = "contact";
var dbPassword = "flyingwhale2612";

var connectionString = $"server={dbHost};port=3306;database={dbName};user=root;password={dbPassword}";
builder.Services.AddDbContext<ContactDbContext>(o => o.UseMySQL(connectionString));
// =========================================

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();