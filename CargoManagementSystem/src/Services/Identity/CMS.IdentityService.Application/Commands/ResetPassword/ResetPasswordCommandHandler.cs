using CMS.IdentityService.Domain.Entities;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CMS.IdentityService.Application.Commands.ResetPassword;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ApiResponse<bool>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ResetPasswordCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ApiResponse<bool>> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(command.Email);
        if (user == null)
            throw new NotFoundException("User", command.Email);

        var resetResult = await _userManager.ResetPasswordAsync(user, command.Token, command.NewPassword);
        if (!resetResult.Succeeded)
            return ApiResponse<bool>.Fail("Invalid token or password requirements not met.");

        return ApiResponse<bool>.Ok(true, "Password has been reset successfully.");
    }
}
