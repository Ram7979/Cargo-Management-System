using AutoMapper;
using CMS.BillingService.Application.DTOs;
using CMS.BillingService.Domain.Entities;
using CMS.BillingService.Domain.Interfaces;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.BillingService.Application.Commands.CreateRateCard;

public class CreateRateCardCommandHandler : IRequestHandler<CreateRateCardCommand, ApiResponse<RateCardDto>>
{
    private readonly IRateCardRepository _rateCardRepository;
    private readonly IMapper _mapper;

    public CreateRateCardCommandHandler(IRateCardRepository rateCardRepository, IMapper mapper)
    {
        _rateCardRepository = rateCardRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<RateCardDto>> Handle(CreateRateCardCommand command, CancellationToken cancellationToken)
    {
        var rateCard = RateCard.Create(
            command.Request.ServiceType,
            command.Request.ZoneFrom,
            command.Request.ZoneTo,
            command.Request.BaseRatePerKg,
            command.Request.FuelSurchargePercent,
            command.Request.HandlingFeeFlat,
            command.Request.TaxPercent,
            command.Request.EffectiveFrom,
            command.Request.EffectiveTo);

        await _rateCardRepository.AddAsync(rateCard);

        var dto = _mapper.Map<RateCardDto>(rateCard);
        return ApiResponse<RateCardDto>.Ok(dto, "Rate card created successfully.");
    }
}
