using AutoMapper;
using CMS.NotificationService.Application.DTOs;
using CMS.NotificationService.Application.Jobs;
using CMS.NotificationService.Domain.Entities;
using CMS.NotificationService.Domain.Enums;
using CMS.NotificationService.Domain.Interfaces;
using CMS.Shared.Responses;
using Hangfire;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CMS.NotificationService.Application.Commands.QueueNotification;

public class QueueNotificationCommandHandler : IRequestHandler<QueueNotificationCommand, ApiResponse<NotificationDto>>
{
    private readonly INotificationRepository _repository;
    private readonly INotificationPreferenceRepository _preferenceRepository;
    private readonly INotificationTemplateRepository _templateRepository;
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IMapper _mapper;
    private readonly ILogger<QueueNotificationCommandHandler> _logger;

    public QueueNotificationCommandHandler(
        INotificationRepository repository,
        INotificationPreferenceRepository preferenceRepository,
        INotificationTemplateRepository templateRepository,
        IBackgroundJobClient backgroundJobClient,
        IMapper mapper,
        ILogger<QueueNotificationCommandHandler> logger)
    {
        _repository = repository;
        _preferenceRepository = preferenceRepository;
        _templateRepository = templateRepository;
        _backgroundJobClient = backgroundJobClient;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<NotificationDto>> Handle(QueueNotificationCommand command, CancellationToken cancellationToken)
    {
        var channel = Enum.Parse<NotificationChannel>(command.Request.Channel, ignoreCase: true);

        // Check customer preferences — skip if opted out
        var prefs = await _preferenceRepository.GetByRecipientIdAsync(command.Request.RecipientId);
        if (prefs != null)
        {
            if (!prefs.IsChannelEnabled(command.Request.Channel))
            {
                _logger.LogInformation(
                    "Skipping {Channel} notification for recipient {RecipientId} — channel disabled by preference",
                    command.Request.Channel, command.Request.RecipientId);
                return ApiResponse<NotificationDto>.Ok(null!, "Notification skipped — channel disabled by recipient preference.");
            }

            if (!string.IsNullOrWhiteSpace(command.Request.EventType) &&
                prefs.IsEventTypeOptedOut(command.Request.EventType))
            {
                _logger.LogInformation(
                    "Skipping {EventType} notification for recipient {RecipientId} — event type opted out",
                    command.Request.EventType, command.Request.RecipientId);
                return ApiResponse<NotificationDto>.Ok(null!, "Notification skipped — event type opted out by recipient.");
            }
        }

        // Resolve subject/body from template if available
        var subject = command.Request.Subject;
        var body = command.Request.Body;

        if (!string.IsNullOrWhiteSpace(command.Request.EventType))
        {
            var template = await _templateRepository.GetByEventTypeAndChannelAsync(command.Request.EventType, channel);
            if (template != null && template.IsActive)
            {
                var variables = command.Request.TemplateVariables ?? new Dictionary<string, string>();
                subject = template.RenderSubject(variables);
                body = template.RenderBody(variables);
            }
        }

        var record = NotificationRecord.Create(
            command.Request.RecipientId,
            channel,
            command.Request.Recipient,
            subject,
            body,
            command.Request.EventType);

        await _repository.AddAsync(record);

        // Enqueue the appropriate Hangfire job
        switch (channel)
        {
            case NotificationChannel.Email:
                _backgroundJobClient.Enqueue<SendEmailJob>(job => job.Execute(record.Id));
                break;
            case NotificationChannel.SMS:
                _backgroundJobClient.Enqueue<SendSmsJob>(job => job.Execute(record.Id));
                break;
            case NotificationChannel.Push:
                _backgroundJobClient.Enqueue<SendPushJob>(job => job.Execute(record.Id));
                break;
        }

        var dto = _mapper.Map<NotificationDto>(record);
        return ApiResponse<NotificationDto>.Ok(dto, "Notification queued successfully.");
    }
}
