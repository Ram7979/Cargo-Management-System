using CMS.NotificationService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.NotificationService.Application.Queries.GetPreference;

public record GetPreferenceQuery(string RecipientId) : IRequest<ApiResponse<NotificationPreferenceDto>>;
