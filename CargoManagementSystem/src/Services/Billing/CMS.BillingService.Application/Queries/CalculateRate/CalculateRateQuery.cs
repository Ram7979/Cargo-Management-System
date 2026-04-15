using CMS.BillingService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.BillingService.Application.Queries.CalculateRate;

public record CalculateRateQuery(
    decimal WeightKg,
    decimal VolumeCbm,
    string ServiceType,
    string OriginCountry,
    string DestinationCountry,
    string? CargoType = null) : IRequest<ApiResponse<RateCalculationResult>>;
