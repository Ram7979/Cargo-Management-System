using CMS.IdentityService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.IdentityService.Application.Commands.RegisterUser;

public class RegisterUserCommand : IRequest<ApiResponse<UserDto>>
{
    public RegisterUserRequest Request { get; }
    public string ActorId { get; }
    public string IpAddress { get; }

    public RegisterUserCommand(RegisterUserRequest request, string actorId, string ipAddress)
    {
        Request = request;
        ActorId = actorId;
        IpAddress = ipAddress;
    }
}
