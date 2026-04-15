using AutoMapper;
using CMS.ShipmentService.Application.DTOs;
using CMS.ShipmentService.Domain.Entities;
using CMS.ShipmentService.Domain.ValueObjects;

namespace CMS.ShipmentService.Application.Mappings;

public class ShipmentMappingProfile : Profile
{
    public ShipmentMappingProfile()
    {
        CreateMap<Shipment, ShipmentDto>()
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.CargoType, opt => opt.MapFrom(s => s.CargoType.ToString()))
            .ForMember(d => d.PaymentMode, opt => opt.MapFrom(s => s.PaymentMode.ToString()))
            .ForMember(d => d.LastFailureReason, opt => opt.MapFrom(s => s.LastFailureReason.HasValue ? s.LastFailureReason.Value.ToString() : null));

        CreateMap<GpsCoordinate, GpsCoordinateDto>();

        CreateMap<ShipmentStatusHistory, ShipmentStatusHistoryDto>()
            .ForMember(d => d.FromStatus, opt => opt.MapFrom(s => s.FromStatus.ToString()))
            .ForMember(d => d.ToStatus, opt => opt.MapFrom(s => s.ToStatus.ToString()))
            .ForMember(d => d.Location, opt => opt.MapFrom(s => s.Location));
    }
}
