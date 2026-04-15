using AutoMapper;
using CMS.BillingService.Application.DTOs;
using CMS.BillingService.Domain.Entities;

namespace CMS.BillingService.Application.Mappings;

public class BillingMappingProfile : Profile
{
    public BillingMappingProfile()
    {
        CreateMap<Invoice, InvoiceDto>()
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()));

        CreateMap<Payment, PaymentDto>()
            .ForMember(d => d.Method, opt => opt.MapFrom(s => s.Method.ToString()));

        CreateMap<RateCard, RateCardDto>();
    }
}
