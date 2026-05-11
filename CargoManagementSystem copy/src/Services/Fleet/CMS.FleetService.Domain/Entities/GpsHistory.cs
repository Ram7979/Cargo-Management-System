using CMS.FleetService.Domain.ValueObjects;
using CMS.Shared.Entities;

namespace CMS.FleetService.Domain.Entities;

public class GpsHistory : BaseEntity
{
    public Guid VehicleId { get; private set; }
    public Guid? AssignmentId { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public DateTime RecordedAt { get; private set; }

    private GpsHistory() { }

    public static GpsHistory Create(Guid vehicleId, Guid? assignmentId, double latitude, double longitude, DateTime recordedAt)
    {
        return new GpsHistory
        {
            VehicleId = vehicleId,
            AssignmentId = assignmentId,
            Latitude = latitude,
            Longitude = longitude,
            RecordedAt = recordedAt
        };
    }
}
