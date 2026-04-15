using AutoMapper;
using CMS.NotificationService.Application.DTOs;
using CMS.NotificationService.Application.Jobs;
using CMS.NotificationService.Domain.Entities;
using CMS.NotificationService.Domain.Enums;
using CMS.NotificationService.Domain.Interfaces;
using CMS.Shared.Responses;
using Hangfire;
using MediatR;

namespace CMS.NotificationService.Application.Commands.QueueNotification;

public class QueueNotificationCommandHandler : IRequestHandler<QueueNotificationCommand, ApiResponse<NotificationDto>>
{
    private readonly INotificationRepository _repository;
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IMapper _mapper;

    public QueueNotificationCommandHandler(
        INotificationRepository repository,
        IBackgroundJobClient backgroundJobClient,
        IMapper mapper)
    {
        _repository = repository;
        _backgroundJobClient = backgroundJobClient;
        _mapper = mapper;
    }

    public async Task<ApiResponse<NotificationDto>> Handle(QueueNotificationCommand command, CancellationToken cancellationToken)
    {
        var channel = Enum.Parse<NotificationChannel>(command.Request.Channel, ignoreCase: true);

        var record = NotificationRecord.Create(
            command.Request.RecipientId,
            channel,
            command.Request.Recipient,
            command.Request.Subject,
            command.Request.Body,
            command.Request.EventType);

        await _repository.AddAsync(record);

        if (channel == NotificationChannel.Email)
            _backgroundJobClient.Enqueue<SendEmailJob>(job => job.Execute(record.Id));
        else
            _backgroundJobClient.Enqueue<SendSmsJob>(job => job.Execute(record.Id));

        var dto = _mapper.Map<NotificationDto>(record);
        return ApiResponse<NotificationDto>.Ok(dto, "Notification queued successfully.");
    }
}
