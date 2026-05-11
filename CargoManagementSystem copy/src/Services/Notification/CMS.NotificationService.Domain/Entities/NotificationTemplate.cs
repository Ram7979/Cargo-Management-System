using CMS.NotificationService.Domain.Enums;
using CMS.Shared.Entities;

namespace CMS.NotificationService.Domain.Entities;

public class NotificationTemplate : BaseEntity
{
    public string EventType { get; private set; } = string.Empty;
    public NotificationChannel Channel { get; private set; }
    public string SubjectTemplate { get; private set; } = string.Empty;
    public string BodyTemplate { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;

    private NotificationTemplate() { }

    public static NotificationTemplate Create(
        string eventType,
        NotificationChannel channel,
        string subjectTemplate,
        string bodyTemplate)
    {
        return new NotificationTemplate
        {
            EventType = eventType,
            Channel = channel,
            SubjectTemplate = subjectTemplate,
            BodyTemplate = bodyTemplate,
            IsActive = true
        };
    }

    /// <summary>
    /// Renders the subject by replacing {Key} placeholders with values from the dictionary.
    /// </summary>
    public string RenderSubject(Dictionary<string, string> variables)
        => Render(SubjectTemplate, variables);

    /// <summary>
    /// Renders the body by replacing {Key} placeholders with values from the dictionary.
    /// </summary>
    public string RenderBody(Dictionary<string, string> variables)
        => Render(BodyTemplate, variables);

    private static string Render(string template, Dictionary<string, string> variables)
    {
        var result = template;
        foreach (var kv in variables)
            result = result.Replace($"{{{kv.Key}}}", kv.Value, StringComparison.OrdinalIgnoreCase);
        return result;
    }

    public void Update(string subjectTemplate, string bodyTemplate, bool isActive)
    {
        SubjectTemplate = subjectTemplate;
        BodyTemplate = bodyTemplate;
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }
}
