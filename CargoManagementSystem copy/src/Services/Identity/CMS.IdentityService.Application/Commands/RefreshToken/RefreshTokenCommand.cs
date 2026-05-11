using CMS.IdentityService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.IdentityService.Application.Commands.RefreshToken;

public class RefreshTokenCommand : IRequest<ApiResponse<LoginResponse>>
{
    public string Token { get; }

    public RefreshTokenCommand(string token)
    {
        Token = token;
    }
}
