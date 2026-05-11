using CMS.NotificationService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.NotificationService.Application.Queries.GetNotifications;

public record GetNotificationsQuery(
    int Page,
    int PageSize,
    string? Status = null,
    string? Channel = null,
    string? RecipientId = null,
    string? EventType = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null)
    : IRequest<ApiResponse<PagedResponse<NotificationDto>>>;
