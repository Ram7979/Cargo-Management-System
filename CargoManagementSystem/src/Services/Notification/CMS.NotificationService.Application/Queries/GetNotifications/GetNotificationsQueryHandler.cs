using AutoMapper;
using CMS.NotificationService.Application.DTOs;
using CMS.NotificationService.Domain.Interfaces;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.NotificationService.Application.Queries.GetNotifications;

public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, PagedResponse<NotificationDto>>
{
    private readonly INotificationRepository _repository;
    private readonly IMapper _mapper;

    public GetNotificationsQueryHandler(INotificationRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PagedResponse<NotificationDto>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _repository.GetPagedAsync(request.Page, request.PageSize);
        var dtos = _mapper.Map<IEnumerable<NotificationDto>>(items);
        return PagedResponse<NotificationDto>.Ok(dtos, request.Page, request.PageSize, totalCount);
    }
}
