using CMS.FleetService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.FleetService.Application.Queries.GetAvailableVehicles;

public record GetAvailableVehiclesQuery(decimal? MinCapacityKg = null, string? Type = null) : IRequest<ApiResponse<IEnumerable<VehicleDto>>>;
