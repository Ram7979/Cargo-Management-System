using AutoMapper;
using CMS.IdentityService.Application.DTOs;
using CMS.IdentityService.Domain.Entities;

namespace CMS.IdentityService.Application.Mappings;

public class IdentityMappingProfile : Profile
{
    public IdentityMappingProfile()
    {
        // Map the old User entity (kept for compatibility)
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles.Select(r => r.RoleName)));

        // Map the new ASP.NET Identity ApplicationUser
        CreateMap<ApplicationUser, UserDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email ?? string.Empty))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.Roles, opt => opt.Ignore()); // Roles loaded separately via UserManager

        CreateMap<AuditLog, AuditLogDto>();
    }
}
