using AutoMapper;
using CMS.NotificationService.Application.DTOs;
using CMS.NotificationService.Domain.Interfaces;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.NotificationService.Application.Queries.GetFailedNotifications;

public class GetFailedNotificationsQueryHandler : IRequestHandler<GetFailedNotificationsQuery, ApiResponse<IEnumerable<NotificationDto>>>
{
    private readonly INotificationRepository _repository;
    private readonly IMapper _mapper;

    public GetFailedNotificationsQueryHandler(INotificationRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<NotificationDto>>> Handle(GetFailedNotificationsQuery request, CancellationToken cancellationToken)
    {
        var failed = await _repository.GetFailedAsync();
        var dtos = _mapper.Map<IEnumerable<NotificationDto>>(failed);
        return ApiResponse<IEnumerable<NotificationDto>>.Ok(dtos);
    }
}
