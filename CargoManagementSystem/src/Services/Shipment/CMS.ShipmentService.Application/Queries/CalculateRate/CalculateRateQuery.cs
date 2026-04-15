using CMS.ShipmentService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.ShipmentService.Application.Queries.CalculateRate;

public record CalculateRateQuery(
    decimal WeightKg,
    decimal VolumeCbm,
    string ServiceType,
    string OriginCountry,
    string DestinationCountry) : IRequest<ApiResponse<RateCalculationDto>>;
