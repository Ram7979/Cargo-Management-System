using System.Security.Cryptography;
using System.Text;
using CMS.IdentityService.Application.DTOs;
using CMS.IdentityService.Application.Interfaces;
using CMS.IdentityService.Domain.Entities;
using CMS.IdentityService.Domain.Exceptions;
using CMS.IdentityService.Domain.Interfaces;
using CMS.Shared.Responses;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CMS.IdentityService.Application.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, ApiResponse<LoginResponse>>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtService _jwtService;

    public RefreshTokenCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        UserManager<ApplicationUser> userManager,
        IJwtService jwtService)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userManager = userManager;
        _jwtService = jwtService;
    }

    public async Task<ApiResponse<LoginResponse>> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var existingToken = await _refreshTokenRepository.GetByTokenAsync(command.Token);

        if (existingToken == null || !existingToken.IsActive())
        {
            if (existingToken != null)
                await _refreshTokenRepository.RevokeAllInFamilyAsync(existingToken.TokenFamily);

            throw new TokenExpiredException("The refresh token is invalid or has expired.");
        }

        var user = await _userManager.FindByIdAsync(existingToken.UserId);
        if (user == null)
            throw new TokenExpiredException("The refresh token is invalid.");

        existingToken.MarkUsed();
        await _refreshTokenRepository.UpdateAsync(existingToken);

        var roles = await _userManager.GetRolesAsync(user);
        var newAccessToken = _jwtService.GenerateAccessToken(user, roles);
        var rawNewRefreshToken = _jwtService.GenerateRefreshToken();

        var newRefreshToken = Domain.Entities.RefreshToken.Create(
            user.Id,
            rawNewRefreshToken,
            existingToken.TokenFamily,
            DateTime.UtcNow.AddDays(7));

        await _refreshTokenRepository.AddAsync(newRefreshToken);

        var response = new LoginResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = rawNewRefreshToken,
            User = new UserDto 
            { 
                Id = user.Id, 
                Email = user.Email!, 
                FirstName = user.FirstName, 
                LastName = user.LastName 
            }
        };

        return ApiResponse<LoginResponse>.Ok(response, "Token refreshed successfully.");
    }
}
