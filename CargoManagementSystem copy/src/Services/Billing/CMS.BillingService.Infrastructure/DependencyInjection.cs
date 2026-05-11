using CMS.BillingService.Application.Interfaces;
using CMS.BillingService.Domain.Interfaces;
using CMS.BillingService.Infrastructure.Persistence;
using CMS.BillingService.Infrastructure.Persistence.Repositories;
using CMS.BillingService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CMS.BillingService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddBillingInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<BillingDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("BillingDb")));

        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IRateCardRepository, RateCardRepository>();
        services.AddScoped<IBlobService, AzureBlobService>();

        services.AddHttpClient<INotificationServiceClient, NotificationServiceClient>(client =>
        {
            client.BaseAddress = new Uri(configuration["NotificationService:BaseUrl"] ?? "http://localhost:5007");
            client.Timeout = TimeSpan.FromSeconds(5);
        });

        services.AddHttpClient<ICustomerServiceClient, CustomerServiceClient>(client =>
        {
            client.BaseAddress = new Uri(configuration["CustomerService:BaseUrl"] ?? "http://localhost:5003");
            client.Timeout = TimeSpan.FromSeconds(5);
        });

        return services;
    }
}
