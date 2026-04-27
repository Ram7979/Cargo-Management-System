using AutoMapper;
using CMS.IdentityService.Application.DTOs;
using CMS.IdentityService.Domain.Interfaces;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.IdentityService.Application.Queries.GetUsers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PagedResponse<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUsersQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<PagedResponse<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var allUsers = await _userRepository.GetAllAsync();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLowerInvariant();
            allUsers = allUsers.Where(u =>
                u.Email.ToLowerInvariant().Contains(search) ||
                u.FirstName.ToLowerInvariant().Contains(search) ||
                u.LastName.ToLowerInvariant().Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(request.Role))
        {
            var role = request.Role.ToLowerInvariant();
            allUsers = allUsers.Where(u =>
                u.Roles.Any(r => r.RoleName.ToLowerInvariant() == role));
        }

        if (request.IsActive.HasValue)
            allUsers = allUsers.Where(u => u.IsActive == request.IsActive.Value);

        var userList = allUsers.ToList();
        var totalCount = userList.Count;

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 20 : request.PageSize > 100 ? 100 : request.PageSize;

        var paged = userList
            .OrderBy(u => u.Email)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var dtos = _mapper.Map<IEnumerable<UserDto>>(paged);
        return PagedResponse<UserDto>.Ok(dtos, page, pageSize, totalCount);
    }
}
