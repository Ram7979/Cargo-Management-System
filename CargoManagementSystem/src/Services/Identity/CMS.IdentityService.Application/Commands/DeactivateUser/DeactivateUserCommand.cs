using CMS.Shared.Responses;
using MediatR;

namespace CMS.IdentityService.Application.Commands.DeactivateUser;

public record DeactivateUserCommand(
    Guid UserId,
    string ActorId,
    string IpAddress) : IRequest<ApiResponse<bool>>;
