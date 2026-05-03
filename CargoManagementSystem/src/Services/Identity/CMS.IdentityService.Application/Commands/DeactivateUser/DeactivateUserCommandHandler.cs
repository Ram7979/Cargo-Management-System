using CMS.IdentityService.Domain.Entities;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CMS.IdentityService.Application.Commands.DeactivateUser;

public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand, ApiResponse<bool>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public DeactivateUserCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ApiResponse<bool>> Handle(DeactivateUserCommand command, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(command.UserId.ToString());
        if (user == null)
            throw new NotFoundException("User", command.UserId);

        user.IsActive = false;
        var result = await _userManager.UpdateAsync(user);
        
        if (!result.Succeeded)
            throw new Exception("Failed to deactivate user.");

        return ApiResponse<bool>.Ok(true, "User deactivated successfully.");
    }
}
