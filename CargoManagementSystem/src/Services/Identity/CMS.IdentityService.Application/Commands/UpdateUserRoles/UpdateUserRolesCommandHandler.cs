using AutoMapper;
using CMS.IdentityService.Application.DTOs;
using CMS.IdentityService.Domain.Entities;
using CMS.IdentityService.Domain.Interfaces;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.IdentityService.Application.Commands.UpdateUserRoles;

public class UpdateUserRolesCommandHandler : IRequestHandler<UpdateUserRolesCommand, ApiResponse<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IMapper _mapper;

    public UpdateUserRolesCommandHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IAuditLogRepository auditLogRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _auditLogRepository = auditLogRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<UserDto>> Handle(UpdateUserRolesCommand command, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(command.UserId);
        if (user == null)
            throw new NotFoundException("User", command.UserId);

        var previousRoles = user.Roles.Select(r => r.RoleName).ToList();

        user.UpdateRoles(command.Request.Roles);
        await _userRepository.UpdateAsync(user);

        // Invalidate all active sessions by revoking all refresh tokens for this user
        var userTokens = await _refreshTokenRepository.GetByFamilyAsync(user.Id.ToString());
        foreach (var token in userTokens.Where(t => t.IsActive()))
        {
            await _refreshTokenRepository.RevokeAllInFamilyAsync(token.TokenFamily);
        }

        var auditLog = AuditLog.Create(
            actorId: command.ActorId,
            action: "UpdateUserRoles",
            resourceType: "User",
            resourceId: user.Id.ToString(),
            ipAddress: command.IpAddress,
            payload: $"{{\"previousRoles\":[{string.Join(",", previousRoles.Select(r => $"\"{r}\""))}],\"newRoles\":[{string.Join(",", command.Request.Roles.Select(r => $"\"{r}\""))}]}}");

        await _auditLogRepository.AddAsync(auditLog);

        var userDto = _mapper.Map<UserDto>(user);
        return ApiResponse<UserDto>.Ok(userDto, "User roles updated successfully.");
    }
}
