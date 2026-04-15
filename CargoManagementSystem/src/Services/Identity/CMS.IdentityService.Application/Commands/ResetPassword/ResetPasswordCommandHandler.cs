using CMS.IdentityService.Domain.Interfaces;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.IdentityService.Application.Commands.ResetPassword;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ApiResponse<bool>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public ResetPasswordCommandHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<ApiResponse<bool>> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(command.Email)
            ?? throw new NotFoundException("User", command.Email);

        if (!user.IsPasswordResetTokenValid(command.Token))
            throw new ValidationException(new[] { "Invalid or expired password reset token." });

        // Hash new password with BCrypt cost 12
        var newHash = BCrypt.Net.BCrypt.HashPassword(command.NewPassword, workFactor: 12);
        user.UpdatePassword(newHash);

        await _userRepository.UpdateAsync(user);

        // Revoke all active refresh tokens — force re-login with new password
        var activeTokens = await _refreshTokenRepository.GetByFamilyAsync(user.Id.ToString());
        foreach (var token in activeTokens)
        {
            token.Revoke();
            await _refreshTokenRepository.UpdateAsync(token);
        }

        return ApiResponse<bool>.Ok(true, "Password has been reset successfully. Please log in with your new password.");
    }
}
