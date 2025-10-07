using Microsoft.AspNetCore.SignalR;

namespace GateKeeper.AI.Shared.Hub;

public class AgentsHub : Microsoft.AspNetCore.SignalR.Hub
{
    public async Task NotifyMessage(string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", message);
    }
}
