using CMS.Shared.Entities;

namespace CMS.IdentityService.Domain.Entities;

public class AuditLog : BaseEntity
{
    public string ActorId { get; private set; } = string.Empty;
    public string Action { get; private set; } = string.Empty;
    public string ResourceType { get; private set; } = string.Empty;
    public string ResourceId { get; private set; } = string.Empty;
    public string IpAddress { get; private set; } = string.Empty;
    public string Payload { get; private set; } = string.Empty;
    public DateTime Timestamp { get; private set; }

    private AuditLog() { }

    public static AuditLog Create(
        string actorId,
        string action,
        string resourceType,
        string resourceId,
        string ipAddress,
        string payload)
    {
        return new AuditLog
        {
            ActorId = actorId,
            Action = action,
            ResourceType = resourceType,
            ResourceId = resourceId,
            IpAddress = ipAddress,
            Payload = payload,
            Timestamp = DateTime.UtcNow
        };
    }
}
