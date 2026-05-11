using CMS.CustomerService.Application.Interfaces;
using CMS.CustomerService.Domain.Interfaces;
using CMS.CustomerService.Infrastructure.Persistence;
using CMS.CustomerService.Infrastructure.Persistence.Repositories;
using CMS.CustomerService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace CMS.CustomerService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCustomerInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<CustomerDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("CustomerDb")));

        // Redis — non-blocking if unavailable
        var redisConn = configuration.GetConnectionString("Redis") ?? "localhost:6379";
        var redisOptions = ConfigurationOptions.Parse(redisConn);
        redisOptions.AbortOnConnectFail = false;
        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisOptions));

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IKycDocumentRepository, KycDocumentRepository>();
        services.AddScoped<ICacheService, RedisCacheService>();
        services.AddScoped<IBlobService, AzureBlobService>();

        services.AddHttpClient<IShipmentServiceClient, ShipmentServiceClient>(client =>
        {
            client.BaseAddress = new Uri(configuration["ShipmentService:BaseUrl"] ?? "http://localhost:5002");
            client.Timeout = TimeSpan.FromSeconds(5);
        });

        services.AddHttpClient<INotificationServiceClient, NotificationServiceClient>(client =>
        {
            client.BaseAddress = new Uri(configuration["NotificationService:BaseUrl"] ?? "http://localhost:5007");
            client.Timeout = TimeSpan.FromSeconds(5);
        });

        return services;
    }
}
