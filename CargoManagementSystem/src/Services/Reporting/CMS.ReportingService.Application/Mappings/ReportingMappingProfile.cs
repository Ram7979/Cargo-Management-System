using AutoMapper;
using CMS.ReportingService.Application.DTOs;
using CMS.ReportingService.Domain.ReadModels;

namespace CMS.ReportingService.Application.Mappings;

public class ReportingMappingProfile : Profile
{
    public ReportingMappingProfile()
    {
        CreateMap<ShipmentReadModel, ShipmentReportDto>();
    }
}
