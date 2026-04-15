using CMS.NotificationService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.NotificationService.Application.Queries.GetNotifications;

public class GetNotificationsQuery : IRequest<PagedResponse<NotificationDto>>
{
    public int Page { get; }
    public int PageSize { get; }

    public GetNotificationsQuery(int page, int pageSize)
    {
        Page = page;
        PageSize = pageSize;
    }
}
