using CMS.IdentityService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.IdentityService.Application.Commands.UpdateUser;

public record UpdateUserCommand(
    Guid UserId,
    UpdateUserRequest Request,
    string ActorId,
    string IpAddress) : IRequest<ApiResponse<UserDto>>;
