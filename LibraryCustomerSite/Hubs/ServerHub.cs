using Microsoft.AspNetCore.SignalR;

namespace LibraryCustomerSite.Hubs
{
    public class ServerHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
