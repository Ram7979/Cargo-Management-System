using AutoMapper;
using CMS.IdentityService.Application.DTOs;
using CMS.IdentityService.Domain.Entities;
using CMS.Shared.Responses;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CMS.IdentityService.Application.Queries.GetUsers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PagedResponse<UserDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public GetUsersQueryHandler(UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<PagedResponse<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var query = _userManager.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLowerInvariant();
            query = query.Where(u =>
                u.Email!.ToLower().Contains(search) ||
                u.FirstName.ToLower().Contains(search) ||
                u.LastName.ToLower().Contains(search));
        }

        if (request.IsActive.HasValue)
            query = query.Where(u => u.IsActive == request.IsActive.Value);

        // Roles are tricky with IQueryable and UserManager, usually requires join or filtering after fetching
        // For simplicity in this fix, we fetch and then filter if role is provided
        var users = query.ToList();

        if (!string.IsNullOrWhiteSpace(request.Role))
        {
            var filtered = new List<ApplicationUser>();
            foreach (var user in users)
            {
                if (await _userManager.IsInRoleAsync(user, request.Role))
                    filtered.Add(user);
            }
            users = filtered;
        }

        var totalCount = users.Count;
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 20 : request.PageSize > 100 ? 100 : request.PageSize;

        var paged = users
            .OrderBy(u => u.Email)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var dtos = new List<UserDto>();
        foreach (var user in paged)
        {
            var dto = _mapper.Map<UserDto>(user);
            dto.Roles = await _userManager.GetRolesAsync(user);
            dtos.Add(dto);
        }

        return PagedResponse<UserDto>.Ok(dtos, page, pageSize, totalCount);
    }
}
