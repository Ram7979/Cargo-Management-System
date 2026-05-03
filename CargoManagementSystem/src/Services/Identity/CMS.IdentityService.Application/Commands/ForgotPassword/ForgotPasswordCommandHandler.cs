using CMS.IdentityService.Domain.Entities;
using CMS.Shared.Responses;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CMS.IdentityService.Application.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, ApiResponse<bool>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ForgotPasswordCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ApiResponse<bool>> Handle(ForgotPasswordCommand command, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(command.Email);
        if (user == null)
        {
            // Don't reveal user existence
            return ApiResponse<bool>.Ok(true, "If an account exists, a reset link has been sent.");
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        
        // In a real app, send email here.
        return ApiResponse<bool>.Ok(true, "Password reset email sent.");
    }
}
