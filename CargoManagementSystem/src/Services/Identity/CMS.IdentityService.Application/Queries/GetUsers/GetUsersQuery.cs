using CMS.IdentityService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.IdentityService.Application.Queries.GetUsers;

public record GetUsersQuery(
    int Page,
    int PageSize,
    string? Search = null,
    string? Role = null,
    bool? IsActive = null) : IRequest<PagedResponse<UserDto>>;
