using CMS.FleetService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.FleetService.Application.Queries.GetAssignment;

public record GetAssignmentQuery(Guid AssignmentId) : IRequest<ApiResponse<AssignmentDto>>;
