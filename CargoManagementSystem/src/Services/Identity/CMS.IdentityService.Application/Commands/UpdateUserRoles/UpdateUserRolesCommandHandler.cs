using AutoMapper;
using CMS.IdentityService.Application.DTOs;
using CMS.IdentityService.Domain.Entities;
using CMS.IdentityService.Domain.Interfaces;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CMS.IdentityService.Application.Commands.UpdateUserRoles;

public class UpdateUserRolesCommandHandler : IRequestHandler<UpdateUserRolesCommand, ApiResponse<UserDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IMapper _mapper;

    public UpdateUserRolesCommandHandler(
        UserManager<ApplicationUser> userManager,
        IRefreshTokenRepository refreshTokenRepository,
        IAuditLogRepository auditLogRepository,
        IMapper mapper)
    {
        _userManager = userManager;
        _refreshTokenRepository = refreshTokenRepository;
        _auditLogRepository = auditLogRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<UserDto>> Handle(UpdateUserRolesCommand command, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(command.UserId.ToString());
        if (user == null)
            throw new NotFoundException("User", command.UserId);

        var currentRoles = await _userManager.GetRolesAsync(user);
        
        // Remove existing roles and add new ones
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        await _userManager.AddToRolesAsync(user, command.Request.Roles);

        // Invalidate all active sessions
        var userTokens = await _refreshTokenRepository.GetByFamilyAsync(user.Id);
        foreach (var token in userTokens.Where(t => t.IsActive()))
        {
            await _refreshTokenRepository.RevokeAllInFamilyAsync(token.TokenFamily);
        }

        var auditLog = AuditLog.Create(
            actorId: command.ActorId,
            action: "UpdateUserRoles",
            resourceType: "User",
            resourceId: user.Id,
            ipAddress: command.IpAddress,
            payload: $"{{\"previousRoles\":[{string.Join(",", currentRoles.Select(r => $"\"{r}\""))}],\"newRoles\":[{string.Join(",", command.Request.Roles.Select(r => $"\"{r}\""))}]}}");

        await _auditLogRepository.AddAsync(auditLog);

        var userDto = _mapper.Map<UserDto>(user);
        userDto.Roles = command.Request.Roles;
        
        return ApiResponse<UserDto>.Ok(userDto, "User roles updated successfully.");
    }
}
