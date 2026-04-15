using AutoMapper;
using CMS.IdentityService.Application.DTOs;
using CMS.IdentityService.Application.Events;
using CMS.IdentityService.Domain.Entities;
using CMS.IdentityService.Domain.Interfaces;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.IdentityService.Application.Commands.RegisterUser;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, ApiResponse<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IAuditLogRepository auditLogRepository,
        IMediator mediator,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _auditLogRepository = auditLogRepository;
        _mediator = mediator;
        _mapper = mapper;
    }

    public async Task<ApiResponse<UserDto>> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        var exists = await _userRepository.ExistsAsync(command.Request.Email);
        if (exists)
            throw new ConflictException($"A user with email '{command.Request.Email}' already exists.");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(command.Request.Password, workFactor: 12);

        var user = User.Create(
            command.Request.Email,
            passwordHash,
            command.Request.FirstName,
            command.Request.LastName);

        user.UpdateRoles(new[] { command.Request.Role });

        await _userRepository.AddAsync(user);

        await _mediator.Publish(new UserRegisteredEvent(
            user.Id,
            user.Email,
            $"{user.FirstName} {user.LastName}"),
            cancellationToken);

        var auditLog = AuditLog.Create(
            actorId: command.ActorId,
            action: "RegisterUser",
            resourceType: "User",
            resourceId: user.Id.ToString(),
            ipAddress: command.IpAddress,
            payload: $"{{\"email\":\"{user.Email}\",\"role\":\"{command.Request.Role}\"}}");

        await _auditLogRepository.AddAsync(auditLog);

        var userDto = _mapper.Map<UserDto>(user);
        return ApiResponse<UserDto>.Ok(userDto, "User registered successfully.");
    }
}
