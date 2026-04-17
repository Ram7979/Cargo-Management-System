using CMS.NotificationService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.NotificationService.Application.Commands.UpdatePreference;

public record UpdatePreferenceCommand(string RecipientId, UpdatePreferenceRequest Request)
    : IRequest<ApiResponse<NotificationPreferenceDto>>;
