using CMS.IdentityService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.IdentityService.Application.Queries.GetUser;

public class GetUserQuery : IRequest<ApiResponse<UserDto>>
{
    public string UserId { get; }

    public GetUserQuery(string userId)
    {
        UserId = userId;
    }
}
