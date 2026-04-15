using CMS.IdentityService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.IdentityService.Application.Commands.UpdateUserRoles;

public class UpdateUserRolesCommand : IRequest<ApiResponse<UserDto>>
{
    public Guid UserId { get; }
    public UpdateUserRolesRequest Request { get; }
    public string ActorId { get; }
    public string IpAddress { get; }

    public UpdateUserRolesCommand(Guid userId, UpdateUserRolesRequest request, string actorId, string ipAddress)
    {
        UserId = userId;
        Request = request;
        ActorId = actorId;
        IpAddress = ipAddress;
    }
}
