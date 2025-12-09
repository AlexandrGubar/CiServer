using CiServer.Core.Interfaces;
using CiServer.Web.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace CiServer.Web.Services;

public class SignalRNotifier : IRealTimeNotifier
{
    private readonly IHubContext<BuildHub> _hubContext;

    public SignalRNotifier(IHubContext<BuildHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendLogAsync(string buildId, string message, string timestamp)
    {
        await _hubContext.Clients.Group(buildId).SendAsync("ReceiveLog", message, timestamp);
    }

    public async Task SendStatusAsync(string buildId, string status)
    {
        await _hubContext.Clients.Group(buildId).SendAsync("BuildFinished", status);
    }
}