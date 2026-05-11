using CMS.FleetService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.FleetService.Application.Queries.GetVehicles;

public record GetVehiclesQuery(int Page = 1, int PageSize = 20, string? Status = null, string? Type = null)
    : IRequest<PagedResponse<VehicleDto>>;
