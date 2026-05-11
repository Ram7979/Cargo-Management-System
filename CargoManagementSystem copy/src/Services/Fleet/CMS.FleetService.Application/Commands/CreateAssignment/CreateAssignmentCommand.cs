using CMS.FleetService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.FleetService.Application.Commands.CreateAssignment;

public record CreateAssignmentCommand(CreateAssignmentRequest Request) : IRequest<ApiResponse<AssignmentDto>>;
