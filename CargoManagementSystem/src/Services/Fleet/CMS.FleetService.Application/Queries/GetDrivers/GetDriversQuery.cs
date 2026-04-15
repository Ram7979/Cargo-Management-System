using CMS.FleetService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.FleetService.Application.Queries.GetDrivers;

public record GetDriversQuery(int Page, int PageSize, string? Status = null) : IRequest<PagedResponse<DriverDto>>;
