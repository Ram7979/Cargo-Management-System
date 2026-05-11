using CMS.FleetService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.FleetService.Application.Commands.UnassignAssignment;

public record UnassignAssignmentCommand(Guid AssignmentId, UnassignRequest Request) : IRequest<ApiResponse<bool>>;
