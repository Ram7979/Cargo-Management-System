using CMS.BillingService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.BillingService.Application.Queries.GetRateCards;

public record GetRateCardsQuery(string? ServiceType = null, bool ActiveOnly = true) : IRequest<ApiResponse<IEnumerable<RateCardDto>>>;
