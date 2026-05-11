using AutoMapper;
using CMS.IdentityService.Application.DTOs;
using CMS.IdentityService.Domain.Entities;
using CMS.Shared.Responses;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CMS.IdentityService.Application.Commands.RegisterUser;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, ApiResponse<UserDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public RegisterUserCommandHandler(UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<ApiResponse<UserDto>> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        var user = new ApplicationUser
        {
            UserName = command.Request.Email,
            Email = command.Request.Email,
            FirstName = command.Request.FirstName,
            LastName = command.Request.LastName,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, command.Request.Password);
        if (!result.Succeeded)
            return ApiResponse<UserDto>.Fail(string.Join(", ", result.Errors.Select(e => e.Description)));

        if (!string.IsNullOrWhiteSpace(command.Request.Role))
            await _userManager.AddToRoleAsync(user, command.Request.Role);
        else
            await _userManager.AddToRoleAsync(user, "Customer");

        var userDto = _mapper.Map<UserDto>(user);
        userDto.Roles = await _userManager.GetRolesAsync(user);

        return ApiResponse<UserDto>.Ok(userDto, "User registered successfully.");
    }
}
