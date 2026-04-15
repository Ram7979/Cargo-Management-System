using CMS.FleetService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.FleetService.Application.Queries.GetAvailableDrivers;

public record GetAvailableDriversQuery : IRequest<ApiResponse<IEnumerable<DriverDto>>>;
