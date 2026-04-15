using CMS.FleetService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.FleetService.Application.Queries.GetDriver;

public record GetDriverQuery(Guid DriverId) : IRequest<ApiResponse<DriverDto>>;
