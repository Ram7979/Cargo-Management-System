using CMS.FleetService.Application.Interfaces;
using CMS.FleetService.Domain.Interfaces;
using CMS.FleetService.Infrastructure.Hubs;
using CMS.FleetService.Infrastructure.Persistence;
using CMS.FleetService.Infrastructure.Persistence.Repositories;
using CMS.FleetService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace CMS.FleetService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddFleetInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<FleetDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("FleetDb")));

        services.AddSingleton<IConnectionMultiplexer>(
            ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")!));

        services.AddScoped<IVehicleRepository, VehicleRepository>();
        services.AddScoped<IDriverRepository, DriverRepository>();
        services.AddScoped<IAssignmentRepository, AssignmentRepository>();
        services.AddScoped<IMaintenanceLogRepository, MaintenanceLogRepository>();
        services.AddScoped<IGpsHistoryRepository, GpsHistoryRepository>();
        services.AddScoped<ICacheService, RedisCacheService>();

        services.AddHttpClient<IShipmentServiceClient, ShipmentServiceClient>(client =>
        {
            client.BaseAddress = new Uri(configuration["ShipmentService:BaseUrl"]!);
            client.Timeout = TimeSpan.FromSeconds(5);
        });

        services.AddSignalR();
        services.AddHostedService<GpsBroadcastService>();

        return services;
    }
}
