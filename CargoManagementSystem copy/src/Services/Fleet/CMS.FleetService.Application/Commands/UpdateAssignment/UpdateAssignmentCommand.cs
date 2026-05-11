using CMS.FleetService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.FleetService.Application.Commands.UpdateAssignment;

public record UpdateAssignmentCommand(Guid AssignmentId, UpdateAssignmentRequest Request) : IRequest<ApiResponse<AssignmentDto>>;
