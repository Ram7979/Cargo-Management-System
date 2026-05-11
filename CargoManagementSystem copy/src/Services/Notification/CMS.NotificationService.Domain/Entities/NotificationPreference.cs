using CMS.Shared.Entities;

namespace CMS.NotificationService.Domain.Entities;

public class NotificationPreference : BaseEntity
{
    public string RecipientId { get; private set; } = string.Empty;
    public bool EmailEnabled { get; private set; } = true;
    public bool SmsEnabled { get; private set; } = true;
    public bool PushEnabled { get; private set; } = false;

    /// <summary>
    /// Comma-separated list of event types the recipient has opted OUT of.
    /// Empty means all events are enabled.
    /// </summary>
    public string OptedOutEventTypes { get; private set; } = string.Empty;

    private NotificationPreference() { }

    public static NotificationPreference CreateDefault(string recipientId)
    {
        return new NotificationPreference
        {
            RecipientId = recipientId,
            EmailEnabled = true,
            SmsEnabled = true,
            PushEnabled = false,
            OptedOutEventTypes = string.Empty
        };
    }

    public void Update(bool emailEnabled, bool smsEnabled, bool pushEnabled, IEnumerable<string>? optedOutEventTypes = null)
    {
        EmailEnabled = emailEnabled;
        SmsEnabled = smsEnabled;
        PushEnabled = pushEnabled;
        OptedOutEventTypes = optedOutEventTypes != null
            ? string.Join(",", optedOutEventTypes)
            : string.Empty;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsChannelEnabled(string channel)
        => channel.ToLowerInvariant() switch
        {
            "email" => EmailEnabled,
            "sms" => SmsEnabled,
            "push" => PushEnabled,
            _ => false
        };

    public bool IsEventTypeOptedOut(string eventType)
        => !string.IsNullOrWhiteSpace(OptedOutEventTypes) &&
           OptedOutEventTypes.Split(',', StringSplitOptions.RemoveEmptyEntries)
               .Any(e => e.Equals(eventType, StringComparison.OrdinalIgnoreCase));

    public IEnumerable<string> GetOptedOutEventTypes()
        => string.IsNullOrWhiteSpace(OptedOutEventTypes)
            ? Enumerable.Empty<string>()
            : OptedOutEventTypes.Split(',', StringSplitOptions.RemoveEmptyEntries);
}
