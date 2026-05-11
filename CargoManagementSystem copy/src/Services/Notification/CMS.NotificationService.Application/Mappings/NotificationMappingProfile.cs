using AutoMapper;
using CMS.NotificationService.Application.DTOs;
using CMS.NotificationService.Domain.Entities;

namespace CMS.NotificationService.Application.Mappings;

public class NotificationMappingProfile : Profile
{
    public NotificationMappingProfile()
    {
        CreateMap<NotificationRecord, NotificationDto>()
            .ForMember(d => d.Channel, opt => opt.MapFrom(s => s.Channel.ToString()))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()));

        CreateMap<NotificationPreference, NotificationPreferenceDto>()
            .ForMember(d => d.OptedOutEventTypes, opt => opt.MapFrom(s => s.GetOptedOutEventTypes()));
    }
}
