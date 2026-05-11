using CMS.Shared.Responses;
using MediatR;

namespace CMS.IdentityService.Application.Commands.Logout;

public class LogoutCommand : IRequest<ApiResponse<bool>>
{
    public string Token { get; }

    public LogoutCommand(string token)
    {
        Token = token;
    }
}
