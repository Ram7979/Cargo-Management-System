using System.Security.Cryptography;
using System.Text;
using CMS.IdentityService.Domain.Interfaces;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.IdentityService.Application.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, ApiResponse<bool>>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public LogoutCommandHandler(IRefreshTokenRepository refreshTokenRepository)
    {
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<ApiResponse<bool>> Handle(LogoutCommand command, CancellationToken cancellationToken)
    {
        var hashedToken = HashToken(command.Token);
        var token = await _refreshTokenRepository.GetByTokenAsync(hashedToken);

        if (token != null)
            await _refreshTokenRepository.RevokeAllInFamilyAsync(token.TokenFamily);

        return ApiResponse<bool>.Ok(true, "Logged out successfully.");
    }

    private static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(bytes);
    }
}
