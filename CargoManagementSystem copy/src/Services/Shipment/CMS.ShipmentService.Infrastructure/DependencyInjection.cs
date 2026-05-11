using CMS.ShipmentService.Application.Interfaces;
using CMS.ShipmentService.Domain.Interfaces;
using CMS.ShipmentService.Infrastructure.Persistence;
using CMS.ShipmentService.Infrastructure.Persistence.Repositories;
using CMS.ShipmentService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace CMS.ShipmentService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddShipmentInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ShipmentDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("ShipmentDb")));

        services.AddSingleton<IConnectionMultiplexer>(
            ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")!));

        services.AddScoped<IShipmentRepository, ShipmentRepository>();
        services.AddScoped<ICacheService, RedisCacheService>();

        services.AddHttpClient<ICustomerServiceClient, CustomerServiceClient>(client =>
        {
            client.BaseAddress = new Uri(configuration["CustomerService:BaseUrl"] ?? "http://localhost:5003");
            client.Timeout = TimeSpan.FromSeconds(5);
        });

        services.AddHttpClient<INotificationServiceClient, NotificationServiceClient>(client =>
        {
            client.BaseAddress = new Uri(configuration["NotificationService:BaseUrl"] ?? "http://localhost:5007");
            client.Timeout = TimeSpan.FromSeconds(5);
        });

        services.AddHttpClient<IBillingServiceClient, BillingServiceClient>(client =>
        {
            client.BaseAddress = new Uri(configuration["BillingService:BaseUrl"] ?? "http://localhost:5006");
            client.Timeout = TimeSpan.FromSeconds(5);
        });

        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}
