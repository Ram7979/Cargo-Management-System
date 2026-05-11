using CMS.Shared.Responses;
using MediatR;

namespace CMS.NotificationService.Application.Commands.DeleteNotification;

public record DeleteNotificationCommand(Guid NotificationId) : IRequest<ApiResponse<bool>>;
