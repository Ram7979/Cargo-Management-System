using CMS.Shared.Responses;
using MediatR;

namespace CMS.IdentityService.Application.Commands.DeactivateUser;

public record DeactivateUserCommand(
    string UserId,
    string ActorId,
    string IpAddress) : IRequest<ApiResponse<bool>>;
