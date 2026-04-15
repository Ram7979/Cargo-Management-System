using CMS.FleetService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.FleetService.Application.Queries.GetLiveLocations;

public record GetLiveLocationsQuery : IRequest<ApiResponse<IEnumerable<VehicleDto>>>;
