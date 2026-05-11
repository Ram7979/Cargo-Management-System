using CMS.FleetService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.FleetService.Application.Queries.GetDriverActiveAssignment;

public record GetDriverActiveAssignmentQuery(Guid DriverId) : IRequest<ApiResponse<AssignmentDto>>;
