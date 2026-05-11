using CMS.NotificationService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.NotificationService.Application.Commands.QueueNotification;

public class QueueNotificationCommand : IRequest<ApiResponse<NotificationDto>>
{
    public QueueNotificationRequest Request { get; }

    public QueueNotificationCommand(QueueNotificationRequest request)
    {
        Request = request;
    }
}
