using CMS.NotificationService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.NotificationService.Application.Queries.GetFailedNotifications;

public record GetFailedNotificationsQuery : IRequest<ApiResponse<IEnumerable<NotificationDto>>>;
