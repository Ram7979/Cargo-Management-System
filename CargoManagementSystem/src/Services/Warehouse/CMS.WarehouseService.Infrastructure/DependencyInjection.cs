using CMS.WarehouseService.Application.Interfaces;
using CMS.WarehouseService.Domain.Interfaces;
using CMS.WarehouseService.Infrastructure.Persistence;
using CMS.WarehouseService.Infrastructure.Persistence.Repositories;
using CMS.WarehouseService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CMS.WarehouseService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddWarehouseInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<WarehouseDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("WarehouseDb"),
                sqlOptions => sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null)));

        // Repositories
        services.AddScoped<IWarehouseRepository, WarehouseRepository>();
        services.AddScoped<IBinRepository, BinRepository>();
        services.AddScoped<ICargoReceiptRepository, CargoReceiptRepository>();
        services.AddScoped<IDamageReportRepository, DamageReportRepository>();

        // HTTP clients
        services.AddHttpClient<IShipmentServiceClient, ShipmentServiceClient>(client =>
        {
            client.BaseAddress = new Uri(configuration["ShipmentService:BaseUrl"]!);
            client.Timeout = TimeSpan.FromSeconds(5);
        });

        services.AddHttpClient<INotificationServiceClient, NotificationServiceClient>(client =>
        {
            client.BaseAddress = new Uri(configuration["NotificationService:BaseUrl"]!);
            client.Timeout = TimeSpan.FromSeconds(5);
        });

        return services;
    }
}
