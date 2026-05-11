using Microsoft.AspNetCore.SignalR;

namespace CMS.FleetService.Infrastructure.Hubs;

public class GpsHub : Hub
{
    public async Task JoinFleetTracking()
        => await Groups.AddToGroupAsync(Context.ConnectionId, "fleet-tracking");
}
