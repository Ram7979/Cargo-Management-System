using AutoMapper;
using CMS.NotificationService.Application.DTOs;
using CMS.NotificationService.Domain.Entities;
using CMS.NotificationService.Domain.Interfaces;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.NotificationService.Application.Queries.GetPreference;

public class GetPreferenceQueryHandler : IRequestHandler<GetPreferenceQuery, ApiResponse<NotificationPreferenceDto>>
{
    private readonly INotificationPreferenceRepository _preferenceRepository;
    private readonly IMapper _mapper;

    public GetPreferenceQueryHandler(INotificationPreferenceRepository preferenceRepository, IMapper mapper)
    {
        _preferenceRepository = preferenceRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<NotificationPreferenceDto>> Handle(GetPreferenceQuery request, CancellationToken cancellationToken)
    {
        var prefs = await _preferenceRepository.GetByRecipientIdAsync(request.RecipientId)
                    ?? NotificationPreference.CreateDefault(request.RecipientId);

        var dto = _mapper.Map<NotificationPreferenceDto>(prefs);
        return ApiResponse<NotificationPreferenceDto>.Ok(dto);
    }
}
