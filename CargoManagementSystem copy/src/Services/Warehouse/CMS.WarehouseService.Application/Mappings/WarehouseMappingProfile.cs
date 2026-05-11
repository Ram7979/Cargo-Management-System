using AutoMapper;
using CMS.WarehouseService.Application.DTOs;
using CMS.WarehouseService.Domain.Entities;

namespace CMS.WarehouseService.Application.Mappings;

public class WarehouseMappingProfile : Profile
{
    public WarehouseMappingProfile()
    {
        CreateMap<Warehouse, WarehouseDto>()
            .ForMember(d => d.OccupiedBins, opt => opt.MapFrom(s => s.Bins.Count(b => b.IsOccupied && b.IsActive)))
            .ForMember(d => d.AvailableBins, opt => opt.MapFrom(s => s.Bins.Count(b => !b.IsOccupied && b.IsActive)));

        CreateMap<Bin, BinDto>();

        CreateMap<CargoReceipt, CargoReceiptDto>();

        CreateMap<DamageReport, DamageReportDto>()
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()));
    }
}
