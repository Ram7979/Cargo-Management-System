using CMS.IdentityService.Application.DTOs;
using CMS.IdentityService.Application.Interfaces;
using CMS.IdentityService.Domain.Entities;
using CMS.IdentityService.Domain.Exceptions;
using CMS.Shared.Responses;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CMS.IdentityService.Application.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, ApiResponse<LoginResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtService _jwtService;

    public LoginCommandHandler(
        UserManager<ApplicationUser> userManager,
        IJwtService jwtService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
    }

    public async Task<ApiResponse<LoginResponse>> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(command.Request.Email);

        if (user == null || !await _userManager.CheckPasswordAsync(user, command.Request.Password))
            throw new InvalidCredentialsException();

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtService.GenerateAccessToken(user, roles);
        var rawRefreshToken = _jwtService.GenerateRefreshToken();

        // Note: Refresh token storage is handled in controller or can be added here
        
        var response = new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = rawRefreshToken,
            User = new UserDto 
            { 
                Id = user.Id, 
                Email = user.Email!, 
                FirstName = user.FirstName, 
                LastName = user.LastName 
            }
        };

        return ApiResponse<LoginResponse>.Ok(response, "Login successful.");
    }
}
