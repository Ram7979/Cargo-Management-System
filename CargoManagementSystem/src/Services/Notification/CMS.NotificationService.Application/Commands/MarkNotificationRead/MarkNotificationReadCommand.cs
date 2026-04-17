using CMS.Shared.Responses;
using MediatR;

namespace CMS.NotificationService.Application.Commands.MarkNotificationRead;

public record MarkNotificationReadCommand(Guid NotificationId) : IRequest<ApiResponse<bool>>;
