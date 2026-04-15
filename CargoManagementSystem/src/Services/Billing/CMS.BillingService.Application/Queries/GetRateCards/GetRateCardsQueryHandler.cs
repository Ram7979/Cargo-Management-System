using AutoMapper;
using CMS.BillingService.Application.DTOs;
using CMS.BillingService.Domain.Interfaces;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.BillingService.Application.Queries.GetRateCards;

public class GetRateCardsQueryHandler : IRequestHandler<GetRateCardsQuery, ApiResponse<IEnumerable<RateCardDto>>>
{
    private readonly IRateCardRepository _rateCardRepository;
    private readonly IMapper _mapper;

    public GetRateCardsQueryHandler(IRateCardRepository rateCardRepository, IMapper mapper)
    {
        _rateCardRepository = rateCardRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<RateCardDto>>> Handle(GetRateCardsQuery request, CancellationToken cancellationToken)
    {
        var rateCards = await _rateCardRepository.GetAllAsync(request.ServiceType, request.ActiveOnly);
        var dtos = _mapper.Map<IEnumerable<RateCardDto>>(rateCards);
        return ApiResponse<IEnumerable<RateCardDto>>.Ok(dtos);
    }
}
