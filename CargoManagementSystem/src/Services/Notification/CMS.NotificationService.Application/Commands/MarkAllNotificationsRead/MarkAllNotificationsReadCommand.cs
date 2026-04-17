using CMS.Shared.Responses;
using MediatR;

namespace CMS.NotificationService.Application.Commands.MarkAllNotificationsRead;

public record MarkAllNotificationsReadCommand(string RecipientId) : IRequest<ApiResponse<int>>;
