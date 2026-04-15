using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using CMS.IdentityService.Application.DTOs;
using CMS.IdentityService.Application.Interfaces;
using CMS.IdentityService.Domain.Entities;
using CMS.IdentityService.Domain.Exceptions;
using CMS.IdentityService.Domain.Interfaces;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.IdentityService.Application.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, ApiResponse<LoginResponse>>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IMapper _mapper;

    public RefreshTokenCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUserRepository userRepository,
        IJwtService jwtService,
        IMapper mapper)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
        _jwtService = jwtService;
        _mapper = mapper;
    }

    public async Task<ApiResponse<LoginResponse>> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var hashedToken = HashToken(command.Token);
        var existingToken = await _refreshTokenRepository.GetByTokenAsync(hashedToken);

        if (existingToken == null || !existingToken.IsActive())
        {
            // Replay attack detected — revoke entire family if token exists
            if (existingToken != null)
                await _refreshTokenRepository.RevokeAllInFamilyAsync(existingToken.TokenFamily);

            throw new TokenExpiredException("The refresh token is invalid or has expired.");
        }

        var user = await _userRepository.GetByIdAsync(existingToken.UserId);
        if (user == null)
            throw new TokenExpiredException("The refresh token is invalid.");

        // Mark old token as used
        existingToken.MarkUsed();
        await _refreshTokenRepository.UpdateAsync(existingToken);

        // Issue new tokens in the same family
        var newAccessToken = _jwtService.GenerateAccessToken(user);
        var rawNewRefreshToken = _jwtService.GenerateRefreshToken();
        var hashedNewToken = HashToken(rawNewRefreshToken);

        var newRefreshToken = Domain.Entities.RefreshToken.Create(
            user.Id,
            hashedNewToken,
            existingToken.TokenFamily,
            DateTime.UtcNow.AddDays(7));

        await _refreshTokenRepository.AddAsync(newRefreshToken);

        var userDto = _mapper.Map<UserDto>(user);
        var response = new LoginResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = rawNewRefreshToken,
            User = userDto
        };

        return ApiResponse<LoginResponse>.Ok(response, "Token refreshed successfully.");
    }

    private static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(bytes);
    }
}
