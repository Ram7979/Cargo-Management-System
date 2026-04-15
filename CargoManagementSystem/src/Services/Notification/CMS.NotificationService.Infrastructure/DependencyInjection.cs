using CMS.NotificationService.Application.Interfaces;
using CMS.NotificationService.Domain.Interfaces;
using CMS.NotificationService.Infrastructure.Persistence;
using CMS.NotificationService.Infrastructure.Persistence.Repositories;
using CMS.NotificationService.Infrastructure.Services;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CMS.NotificationService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddNotificationInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<NotificationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("NotificationDb")));

        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IEmailService, SendGridEmailService>();
        services.AddScoped<ISmsService, TwilioSmsService>();

        services.AddHangfire(config =>
            config.UseSqlServerStorage(configuration.GetConnectionString("HangfireDb")));

        services.AddHangfireServer();

        return services;
    }
}
