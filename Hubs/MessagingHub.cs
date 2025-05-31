using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace HopIn_Server.Hubs
{
    public class MessagingHub : Hub
    {
        public async Task SendMessage(string chatId, object message)
        {
            await Clients.Group(chatId).SendAsync("ReceiveMessage", message);
        }

        public override async Task OnConnectedAsync()
        {
            var chatId = Context.GetHttpContext()?.Request.Query["chatId"];
            if (!string.IsNullOrEmpty(chatId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
            }

            await base.OnConnectedAsync();
        }
    }
}
