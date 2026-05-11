using CMS.IdentityService.Application.Interfaces;
using CMS.IdentityService.Domain.Interfaces;
using CMS.IdentityService.Infrastructure.Persistence;
using CMS.IdentityService.Infrastructure.Persistence.Repositories;
using CMS.IdentityService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace CMS.IdentityService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<IdentityDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("IdentityDb"),
                sqlOptions => sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null)));

        // Use abortConnect=false so the app starts even if Redis is unavailable
        var redisConnection = configuration.GetConnectionString("Redis") ?? "localhost:6379";
        var configOptions = ConfigurationOptions.Parse(redisConnection);
        configOptions.AbortOnConnectFail = false;
        services.AddSingleton<IConnectionMultiplexer>(
            ConnectionMultiplexer.Connect(configOptions));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<ICacheService, RedisCacheService>();
        services.AddScoped<IJwtService, JwtService>();

        return services;
    }
}
