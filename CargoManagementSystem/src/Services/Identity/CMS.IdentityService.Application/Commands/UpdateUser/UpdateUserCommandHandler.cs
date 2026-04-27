using AutoMapper;
using CMS.IdentityService.Application.DTOs;
using CMS.IdentityService.Domain.Entities;
using CMS.IdentityService.Domain.Interfaces;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.IdentityService.Application.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, ApiResponse<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IMapper _mapper;

    public UpdateUserCommandHandler(
        IUserRepository userRepository,
        IAuditLogRepository auditLogRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _auditLogRepository = auditLogRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<UserDto>> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(command.UserId)
            ?? throw new NotFoundException("User", command.UserId);

        user.UpdateProfile(
            command.Request.FirstName,
            command.Request.LastName);

        await _userRepository.UpdateAsync(user);

        var auditLog = AuditLog.Create(
            actorId: command.ActorId,
            action: "UpdateUser",
            resourceType: "User",
            resourceId: user.Id.ToString(),
            ipAddress: command.IpAddress,
            payload: $"{{\"userId\":\"{user.Id}\",\"updatedFields\":\"profile\"}}");

        await _auditLogRepository.AddAsync(auditLog);

        var dto = _mapper.Map<UserDto>(user);
        return ApiResponse<UserDto>.Ok(dto, "User profile updated successfully.");
    }
}
