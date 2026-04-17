using CMS.NotificationService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.NotificationService.Application.Queries.GetNotification;

public record GetNotificationQuery(Guid NotificationId) : IRequest<ApiResponse<NotificationDto>>;
