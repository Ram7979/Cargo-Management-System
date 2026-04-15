using CMS.IdentityService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.IdentityService.Application.Commands.Login;

public class LoginCommand : IRequest<ApiResponse<LoginResponse>>
{
    public LoginRequest Request { get; }
    public string IpAddress { get; }

    public LoginCommand(LoginRequest request, string ipAddress)
    {
        Request = request;
        IpAddress = ipAddress;
    }
}
