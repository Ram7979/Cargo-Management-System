using CMS.FleetService.Application.Interfaces;
using CMS.FleetService.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CMS.FleetService.Infrastructure.Services;

public class GpsBroadcastService : BackgroundService
{
    private readonly IHubContext<GpsHub> _hubContext;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<GpsBroadcastService> _logger;

    public GpsBroadcastService(
        IHubContext<GpsHub> hubContext,
        IServiceScopeFactory scopeFactory,
        ILogger<GpsBroadcastService> logger)
    {
        _hubContext = hubContext;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await BroadcastLocationsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error broadcasting GPS locations.");
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }

    private async Task BroadcastLocationsAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();

        var locations = await cacheService.GetByPatternAsync<object>("fleet:live:*");

        await _hubContext.Clients.Group("fleet-tracking")
            .SendAsync("LiveLocations", locations);
    }
}
