using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using NotificationWebApi.Models;

namespace NotificationWebApi.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendNotification(string receiverId, Notification notification)
        {
            await Clients.User(receiverId).SendAsync("ReceiveNotification", notification);
        }
    }

}