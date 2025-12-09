using Microsoft.AspNetCore.SignalR;

namespace CiServer.Web.Hubs;

public class BuildHub : Hub
{
    public async Task JoinBuildGroup(string buildId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, buildId);
    }
}