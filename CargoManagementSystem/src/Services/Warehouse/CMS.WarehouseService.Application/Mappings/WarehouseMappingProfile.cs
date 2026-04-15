using AutoMapper;
using CMS.WarehouseService.Application.DTOs;
using CMS.WarehouseService.Domain.Entities;

namespace CMS.WarehouseService.Application.Mappings;

public class WarehouseMappingProfile : Profile
{
    public WarehouseMappingProfile()
    {
        CreateMap<Warehouse, WarehouseDto>();
        CreateMap<Bin, BinDto>();
        CreateMap<CargoReceipt, CargoReceiptDto>();
    }
}
