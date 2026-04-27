using CMS.BillingService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.BillingService.Application.Commands.UpdateRateCard;

public record UpdateRateCardCommand(
    Guid RateCardId,
    UpdateRateCardRequest Request) : IRequest<ApiResponse<RateCardDto>>;
