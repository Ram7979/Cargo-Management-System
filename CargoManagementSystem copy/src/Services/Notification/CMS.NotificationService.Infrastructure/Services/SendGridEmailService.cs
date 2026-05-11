using CMS.NotificationService.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace CMS.NotificationService.Infrastructure.Services;

public class SendGridEmailService : IEmailService
{
    private readonly string _apiKey;
    private readonly string _fromEmail;
    private readonly ILogger<SendGridEmailService> _logger;

    public SendGridEmailService(IConfiguration configuration, ILogger<SendGridEmailService> logger)
    {
        _apiKey = configuration["SendGrid:ApiKey"] ?? string.Empty;
        _fromEmail = configuration["SendGrid:FromEmail"] ?? "noreply@cms.local";
        _logger = logger;
    }

    public async Task<bool> SendAsync(string to, string subject, string body)
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
        {
            _logger.LogInformation("SendGrid not configured. Simulating email to {To}: {Subject}", to, subject);
            return true;
        }

        try
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress(_fromEmail);
            var toAddress = new EmailAddress(to);
            var msg = MailHelper.CreateSingleEmail(from, toAddress, subject, body, body);
            var response = await client.SendEmailAsync(msg);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", to);
            return false;
        }
    }
}
