using CMS.BillingService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.BillingService.Application.Commands.CreateRateCard;

public record CreateRateCardCommand(CreateRateCardRequest Request) : IRequest<ApiResponse<RateCardDto>>;
