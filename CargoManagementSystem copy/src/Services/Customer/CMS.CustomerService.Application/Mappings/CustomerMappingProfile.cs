using AutoMapper;
using CMS.CustomerService.Application.DTOs;
using CMS.CustomerService.Domain.Entities;

namespace CMS.CustomerService.Application.Mappings;

public class CustomerMappingProfile : Profile
{
    public CustomerMappingProfile()
    {
        CreateMap<Customer, CustomerDto>()
            .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type.ToString()))
            .ForMember(d => d.KycDocuments, opt => opt.MapFrom(s => s.KycDocuments));

        CreateMap<KycDocument, KycDocumentDto>();
    }
}
