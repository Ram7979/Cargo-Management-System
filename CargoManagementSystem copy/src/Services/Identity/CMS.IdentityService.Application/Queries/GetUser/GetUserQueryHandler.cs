using AutoMapper;
using CMS.IdentityService.Application.DTOs;
using CMS.IdentityService.Domain.Entities;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CMS.IdentityService.Application.Queries.GetUser;

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, ApiResponse<UserDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public GetUserQueryHandler(UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<ApiResponse<UserDto>> Handle(GetUserQuery query, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(query.UserId.ToString());
        if (user == null)
            throw new NotFoundException("User", query.UserId);

        var userDto = _mapper.Map<UserDto>(user);
        userDto.Roles = await _userManager.GetRolesAsync(user);
        
        return ApiResponse<UserDto>.Ok(userDto);
    }
}
