using System.Security.Cryptography;
using CMS.IdentityService.Domain.Interfaces;
using CMS.Shared.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CMS.IdentityService.Application.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, ApiResponse<bool>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ForgotPasswordCommandHandler> _logger;

    public ForgotPasswordCommandHandler(
        IUserRepository userRepository,
        ILogger<ForgotPasswordCommandHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<ApiResponse<bool>> Handle(ForgotPasswordCommand command, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(command.Email);

        // Always return success to prevent email enumeration attacks
        if (user == null || !user.IsActive)
        {
            _logger.LogInformation("ForgotPassword requested for unknown/inactive email: {Email}", command.Email);
            return ApiResponse<bool>.Ok(true, "If the email exists, a reset link has been sent.");
        }

        // Generate a secure random token
        var tokenBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(tokenBytes);
        var resetToken = Convert.ToBase64String(tokenBytes)
            .Replace("+", "-").Replace("/", "_").Replace("=", ""); // URL-safe

        // Token valid for 1 hour
        user.SetPasswordResetToken(resetToken, DateTime.UtcNow.AddHours(1));
        await _userRepository.UpdateAsync(user);

        // In production: send email with reset link containing the token
        // For now: log the token (replace with email service in production)
        _logger.LogInformation(
            "Password reset token for {Email}: {Token} (expires in 1 hour)",
            user.Email, resetToken);

        return ApiResponse<bool>.Ok(true, "If the email exists, a reset link has been sent.");
    }
}
