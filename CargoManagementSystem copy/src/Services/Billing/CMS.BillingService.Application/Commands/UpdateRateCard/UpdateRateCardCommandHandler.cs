using AutoMapper;
using CMS.BillingService.Application.DTOs;
using CMS.BillingService.Domain.Interfaces;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.BillingService.Application.Commands.UpdateRateCard;

public class UpdateRateCardCommandHandler : IRequestHandler<UpdateRateCardCommand, ApiResponse<RateCardDto>>
{
    private readonly IRateCardRepository _rateCardRepository;
    private readonly IMapper _mapper;

    public UpdateRateCardCommandHandler(IRateCardRepository rateCardRepository, IMapper mapper)
    {
        _rateCardRepository = rateCardRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<RateCardDto>> Handle(UpdateRateCardCommand command, CancellationToken cancellationToken)
    {
        var rateCard = await _rateCardRepository.GetByIdAsync(command.RateCardId)
            ?? throw new NotFoundException("RateCard", command.RateCardId);

        rateCard.Update(
            command.Request.BaseRatePerKg,
            command.Request.FuelSurchargePercent,
            command.Request.HandlingFeeFlat,
            command.Request.TaxPercent,
            command.Request.EffectiveTo);

        await _rateCardRepository.UpdateAsync(rateCard);

        var dto = _mapper.Map<RateCardDto>(rateCard);
        return ApiResponse<RateCardDto>.Ok(dto, "Rate card updated successfully.");
    }
}
