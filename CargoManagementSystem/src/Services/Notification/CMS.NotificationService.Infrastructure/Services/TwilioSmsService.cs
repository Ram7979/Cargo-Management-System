using CMS.NotificationService.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace CMS.NotificationService.Infrastructure.Services;

public class TwilioSmsService : ISmsService
{
    private readonly string _accountSid;
    private readonly string _authToken;
    private readonly string _fromNumber;
    private readonly ILogger<TwilioSmsService> _logger;

    public TwilioSmsService(IConfiguration configuration, ILogger<TwilioSmsService> logger)
    {
        _accountSid = configuration["Twilio:AccountSid"] ?? string.Empty;
        _authToken = configuration["Twilio:AuthToken"] ?? string.Empty;
        _fromNumber = configuration["Twilio:FromNumber"] ?? string.Empty;
        _logger = logger;
    }

    public async Task<bool> SendAsync(string to, string body)
    {
        if (string.IsNullOrWhiteSpace(_accountSid) || string.IsNullOrWhiteSpace(_authToken))
        {
            _logger.LogInformation("Twilio not configured. Simulating SMS to {To}: {Body}", to, body);
            return true;
        }

        try
        {
            TwilioClient.Init(_accountSid, _authToken);
            var message = await MessageResource.CreateAsync(
                body: body,
                from: new Twilio.Types.PhoneNumber(_fromNumber),
                to: new Twilio.Types.PhoneNumber(to));

            return message.ErrorCode == null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send SMS to {To}", to);
            return false;
        }
    }
}
