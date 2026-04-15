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
using DomainRefreshToken = CMS.IdentityService.Domain.Entities.RefreshToken;

namespace CMS.IdentityService.Application.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, ApiResponse<LoginResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IJwtService _jwtService;
    private readonly IMapper _mapper;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IAuditLogRepository auditLogRepository,
        IJwtService jwtService,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _auditLogRepository = auditLogRepository;
        _jwtService = jwtService;
        _mapper = mapper;
    }

    public async Task<ApiResponse<LoginResponse>> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(command.Request.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(command.Request.Password, user.PasswordHash))
            throw new InvalidCredentialsException();

        var accessToken = _jwtService.GenerateAccessToken(user);
        var rawRefreshToken = _jwtService.GenerateRefreshToken();
        var tokenFamily = _jwtService.GetTokenFamily();

        var hashedToken = HashToken(rawRefreshToken);
        var refreshToken = DomainRefreshToken.Create(
            user.Id,
            hashedToken,
            tokenFamily,
            DateTime.UtcNow.AddDays(7));

        await _refreshTokenRepository.AddAsync(refreshToken);

        var auditLog = AuditLog.Create(
            actorId: user.Id.ToString(),
            action: "Login",
            resourceType: "User",
            resourceId: user.Id.ToString(),
            ipAddress: command.IpAddress,
            payload: $"{{\"outcome\":\"success\"}}");

        await _auditLogRepository.AddAsync(auditLog);

        var userDto = _mapper.Map<UserDto>(user);
        var response = new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = rawRefreshToken,
            User = userDto
        };

        return ApiResponse<LoginResponse>.Ok(response, "Login successful.");
    }

    private static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(bytes);
    }
}
