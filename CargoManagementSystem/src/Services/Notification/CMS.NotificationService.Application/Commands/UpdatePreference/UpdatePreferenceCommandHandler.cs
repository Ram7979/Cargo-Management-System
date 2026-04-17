using AutoMapper;
using CMS.NotificationService.Application.DTOs;
using CMS.NotificationService.Domain.Entities;
using CMS.NotificationService.Domain.Interfaces;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.NotificationService.Application.Commands.UpdatePreference;

public class UpdatePreferenceCommandHandler : IRequestHandler<UpdatePreferenceCommand, ApiResponse<NotificationPreferenceDto>>
{
    private readonly INotificationPreferenceRepository _preferenceRepository;
    private readonly IMapper _mapper;

    public UpdatePreferenceCommandHandler(
        INotificationPreferenceRepository preferenceRepository,
        IMapper mapper)
    {
        _preferenceRepository = preferenceRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<NotificationPreferenceDto>> Handle(UpdatePreferenceCommand command, CancellationToken cancellationToken)
    {
        var existing = await _preferenceRepository.GetByRecipientIdAsync(command.RecipientId);

        if (existing is null)
        {
            existing = NotificationPreference.CreateDefault(command.RecipientId);
            existing.Update(command.Request.EmailEnabled, command.Request.SmsEnabled,
                command.Request.PushEnabled, command.Request.OptedOutEventTypes);
            await _preferenceRepository.AddAsync(existing);
        }
        else
        {
            existing.Update(command.Request.EmailEnabled, command.Request.SmsEnabled,
                command.Request.PushEnabled, command.Request.OptedOutEventTypes);
            await _preferenceRepository.UpdateAsync(existing);
        }

        var dto = _mapper.Map<NotificationPreferenceDto>(existing);
        return ApiResponse<NotificationPreferenceDto>.Ok(dto, "Preferences updated successfully.");
    }
}
