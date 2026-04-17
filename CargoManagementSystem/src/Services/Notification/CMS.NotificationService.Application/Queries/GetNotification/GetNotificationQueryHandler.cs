using AutoMapper;
using CMS.NotificationService.Application.DTOs;
using CMS.NotificationService.Domain.Interfaces;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.NotificationService.Application.Queries.GetNotification;

public class GetNotificationQueryHandler : IRequestHandler<GetNotificationQuery, ApiResponse<NotificationDto>>
{
    private readonly INotificationRepository _repository;
    private readonly IMapper _mapper;

    public GetNotificationQueryHandler(INotificationRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<NotificationDto>> Handle(GetNotificationQuery request, CancellationToken cancellationToken)
    {
        var record = await _repository.GetByIdAsync(request.NotificationId)
            ?? throw new NotFoundException("Notification", request.NotificationId);

        var dto = _mapper.Map<NotificationDto>(record);
        return ApiResponse<NotificationDto>.Ok(dto);
    }
}
