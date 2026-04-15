using CMS.Shared.Responses;
using MediatR;

namespace CMS.FleetService.Application.Queries.GetVehicleRoute;

public record GetVehicleRouteQuery(Guid VehicleId, Guid? AssignmentId = null) : IRequest<ApiResponse<IEnumerable<GpsHistoryDto>>>;

public class GpsHistoryDto
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime RecordedAt { get; set; }
}
