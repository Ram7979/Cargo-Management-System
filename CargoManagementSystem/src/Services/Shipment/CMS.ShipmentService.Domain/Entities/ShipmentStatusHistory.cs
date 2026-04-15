using CMS.Shared.Entities;
using CMS.ShipmentService.Domain.Enums;
using CMS.ShipmentService.Domain.ValueObjects;

namespace CMS.ShipmentService.Domain.Entities;

public class ShipmentStatusHistory : BaseEntity
{
    public Guid ShipmentId { get; private set; }
    public ShipmentStatus FromStatus { get; private set; }
    public ShipmentStatus ToStatus { get; private set; }
    public string ActorId { get; private set; } = string.Empty;
    public string Notes { get; private set; } = string.Empty;
    public DateTime ChangedAt { get; private set; }
    public GpsCoordinate? Location { get; private set; }

    private ShipmentStatusHistory() { }

    public static ShipmentStatusHistory Create(
        Guid shipmentId,
        ShipmentStatus from,
        ShipmentStatus to,
        string actorId,
        string notes,
        GpsCoordinate? location = null)
    {
        return new ShipmentStatusHistory
        {
            ShipmentId = shipmentId,
            FromStatus = from,
            ToStatus = to,
            ActorId = actorId,
            Notes = notes,
            ChangedAt = DateTime.UtcNow,
            Location = location
        };
    }
}
