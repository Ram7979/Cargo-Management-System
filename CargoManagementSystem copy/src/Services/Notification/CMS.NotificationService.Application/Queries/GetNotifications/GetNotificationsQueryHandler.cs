using AutoMapper;
using CMS.NotificationService.Application.DTOs;
using CMS.NotificationService.Domain.Interfaces;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.NotificationService.Application.Queries.GetNotifications;

public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, ApiResponse<PagedResponse<NotificationDto>>>
{
    private readonly INotificationRepository _repository;
    private readonly IMapper _mapper;

    public GetNotificationsQueryHandler(INotificationRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PagedResponse<NotificationDto>>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _repository.GetPagedAsync(
            request.Page, request.PageSize,
            request.Status, request.Channel,
            request.RecipientId, request.EventType,
            request.FromDate, request.ToDate);

        var dtos = _mapper.Map<IEnumerable<NotificationDto>>(items);
        var paged = PagedResponse<NotificationDto>.Ok(dtos, request.Page, request.PageSize, totalCount);
        return ApiResponse<PagedResponse<NotificationDto>>.Ok(paged);
    }
}
