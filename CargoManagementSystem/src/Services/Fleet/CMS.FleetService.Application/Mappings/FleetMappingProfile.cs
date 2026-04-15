using AutoMapper;
using CMS.FleetService.Application.DTOs;
using CMS.FleetService.Domain.Entities;
using CMS.FleetService.Domain.ValueObjects;

namespace CMS.FleetService.Application.Mappings;

public class FleetMappingProfile : Profile
{
    public FleetMappingProfile()
    {
        CreateMap<Vehicle, VehicleDto>()
            .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type.ToString()))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.FuelType, opt => opt.MapFrom(s => s.FuelType.ToString()))
            .ForMember(d => d.GpsTrackingStatus, opt => opt.MapFrom(s => s.GpsTrackingStatus.ToString()));

        CreateMap<Driver, DriverDto>()
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.IsLicenseExpired, opt => opt.MapFrom(s => s.IsLicenseExpired()));

        CreateMap<Assignment, AssignmentDto>()
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()));

        CreateMap<MaintenanceLog, MaintenanceLogDto>();

        CreateMap<GpsCoordinate, GpsCoordinateDto>();
    }
}
