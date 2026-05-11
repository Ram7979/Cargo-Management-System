using AutoMapper;
using CMS.IdentityService.Application.DTOs;
using CMS.IdentityService.Domain.Entities;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CMS.IdentityService.Application.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, ApiResponse<UserDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public UpdateUserCommandHandler(UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<ApiResponse<UserDto>> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(command.UserId);
        if (user == null)
            throw new NotFoundException("User", command.UserId);

        user.FirstName = command.Request.FirstName ?? user.FirstName;
        user.LastName = command.Request.LastName ?? user.LastName;
        
        if (command.Request.IsActive.HasValue)
        {
            user.IsActive = command.Request.IsActive.Value;
        }

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new Exception("Failed to update user.");

        var userDto = _mapper.Map<UserDto>(user);
        userDto.Roles = await _userManager.GetRolesAsync(user);

        return ApiResponse<UserDto>.Ok(userDto, "User updated successfully.");
    }
}
