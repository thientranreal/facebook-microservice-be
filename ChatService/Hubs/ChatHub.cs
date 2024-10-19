using Microsoft.AspNetCore.SignalR;

namespace ChatService.Hubs;

public class ChatHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId} has connected");
    }
    
    public async Task SendMessage(int msgId, int fromId, int sentToId, string message)
    {
        // Send to all clients that connect
        await Clients.All.SendAsync("ReceiveMessage", msgId, fromId, sentToId, message);
    }
}