using System.Net.WebSockets;
using Microsoft.EntityFrameworkCore;
using NotificationWebApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Database Context Dependency Injection
var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
var dbName = Environment.GetEnvironmentVariable("DB_NAME");
var dbPassword = Environment.GetEnvironmentVariable("DB_ROOT_PASSWORD");

var connectionString = $"server={dbHost};port=3306;database={dbName};user=root;password={dbPassword}";
builder.Services.AddDbContext<NotificationDbContext>(o => o.UseMySQL(connectionString));
//=========================================

var app = builder.Build();
// Bật WebSocket với cấu hình
// app.UseWebSockets(new WebSocketOptions
// {
//     KeepAliveInterval = TimeSpan.FromMinutes(2) // thời gian giữ kết nối
// });

// Middleware để xử lý các yêu cầu WebSocket
// app.Use(async (context, next) =>
// {
//     if (context.Request.Path == "/ws")
//     {
//         if (context.WebSockets.IsWebSocketRequest)
//         {
//             var webSocket = await context.WebSockets.AcceptWebSocketAsync();
//             await Echo(context, webSocket);
//         }
//         else
//         {
//             context.Response.StatusCode = 400;
//         }
//     }
//     else
//     {
//         await next();
//     }
// });



// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
//
// static async Task Echo(HttpContext context, WebSocket webSocket)
// {
//     var buffer = new byte[1024 * 4];
//     WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
//
//     while (!result.CloseStatus.HasValue)
//     {
//         await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
//         result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
//     }
//     await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
// }