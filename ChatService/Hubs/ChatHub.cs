using Microsoft.AspNetCore.SignalR;

namespace ChatService.Hubs;

public class ChatHub : Hub
{
    private static readonly Dictionary<string, string> _userConnections = new Dictionary<string, string>();

    public override async Task OnConnectedAsync()
    {
        var userId = Context.GetHttpContext()?.Request.Query["userId"].ToString();
        
        if (!string.IsNullOrEmpty(userId))
        {
            var connectionId = Context.ConnectionId;
        
            // Store the connectionId with userId
            _userConnections[userId] = connectionId;
        }
        
        await base.OnConnectedAsync();
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.GetHttpContext()?.Request.Query["userId"].ToString();
        if (!string.IsNullOrEmpty(userId) && _userConnections.ContainsKey(userId))
        {
            _userConnections.Remove(userId);
        }

        await base.OnDisconnectedAsync(exception);
    }
    
    public async Task SendMessage(int msgId, int fromId, int sentToId, string message)
    {
        // Send to the client
        await Clients.Client(_userConnections[sentToId.ToString()]).SendAsync("ReceiveMessage", msgId, fromId, message);
    }
}