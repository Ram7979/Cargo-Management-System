using CMS.ReportingService.Application.Interfaces;
using CMS.ReportingService.Domain.Interfaces;
using CMS.ReportingService.Infrastructure.Persistence;
using CMS.ReportingService.Infrastructure.Persistence.Repositories;
using CMS.ReportingService.Infrastructure.Services;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace CMS.ReportingService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddReportingInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ReportingDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("ReportingDb")));

        services.AddSingleton<IConnectionMultiplexer>(
            ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")!));

        services.AddScoped<IShipmentReadModelRepository, ShipmentReadModelRepository>();
        services.AddScoped<ICacheService, RedisCacheService>();
        services.AddScoped<IBlobService, AzureBlobService>();

        services.AddHttpClient<IInvoiceServiceClient, InvoiceServiceClient>(client =>
        {
            client.BaseAddress = new Uri(configuration["BillingService:BaseUrl"]!);
            client.Timeout = TimeSpan.FromSeconds(5);
        });

        services.AddHttpClient<ShipmentServiceClient>(client =>
        {
            client.BaseAddress = new Uri(configuration["ShipmentService:BaseUrl"]!);
            client.Timeout = TimeSpan.FromSeconds(5);
        });

        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(configuration.GetConnectionString("HangfireDb"),
                new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));

        services.AddHangfireServer();

        return services;
    }
}
