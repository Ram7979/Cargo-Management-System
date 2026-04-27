using CMS.IdentityService.Domain.Entities;
using CMS.IdentityService.Domain.Interfaces;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.IdentityService.Application.Commands.DeactivateUser;

public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand, ApiResponse<bool>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IAuditLogRepository _auditLogRepository;

    public DeactivateUserCommandHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IAuditLogRepository auditLogRepository)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _auditLogRepository = auditLogRepository;
    }

    public async Task<ApiResponse<bool>> Handle(DeactivateUserCommand command, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(command.UserId)
            ?? throw new NotFoundException("User", command.UserId);

        if (!user.IsActive)
            throw new UnprocessableException($"User '{command.UserId}' is already inactive.");

        user.Deactivate();
        await _userRepository.UpdateAsync(user);

        var activeTokens = await _refreshTokenRepository.GetByFamilyAsync(user.Id.ToString());
        foreach (var token in activeTokens.Where(t => t.IsActive()))
        {
            await _refreshTokenRepository.RevokeAllInFamilyAsync(token.TokenFamily);
        }

        var auditLog = AuditLog.Create(
            actorId: command.ActorId,
            action: "DeactivateUser",
            resourceType: "User",
            resourceId: user.Id.ToString(),
            ipAddress: command.IpAddress,
            payload: $"{{\"userId\":\"{user.Id}\",\"email\":\"{user.Email}\"}}");

        await _auditLogRepository.AddAsync(auditLog);

        return ApiResponse<bool>.Ok(true, $"User '{user.Email}' has been deactivated. All active sessions have been terminated.");
    }
}
