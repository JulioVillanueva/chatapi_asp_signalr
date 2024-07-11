using Microsoft.AspNetCore.SignalR;

namespace demo_chatHub
{
    public class ChatHub : Hub<IChatClient>
    {
        public async Task Broadcast(string someRandomText)
        {
            await Clients.All.ReceiveMessage(@$"{Context.ConnectionId} said: ""{someRandomText}""");
        }
        public override async Task OnConnectedAsync()
        {
            await Clients.All.ReceiveMessage($"{Context.ConnectionId} is now connected");
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Clients.All.ReceiveMessage($"{Context.ConnectionId} is now disconnected");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
