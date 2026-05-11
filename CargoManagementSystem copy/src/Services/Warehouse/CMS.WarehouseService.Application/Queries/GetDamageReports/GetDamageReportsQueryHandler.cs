using AutoMapper;
using CMS.Shared.Responses;
using CMS.WarehouseService.Application.DTOs;
using CMS.WarehouseService.Domain.Interfaces;
using MediatR;

namespace CMS.WarehouseService.Application.Queries.GetDamageReports;

public class GetDamageReportsQueryHandler : IRequestHandler<GetDamageReportsQuery, ApiResponse<IEnumerable<DamageReportDto>>>
{
    private readonly IDamageReportRepository _damageReportRepository;
    private readonly IMapper _mapper;

    public GetDamageReportsQueryHandler(IDamageReportRepository damageReportRepository, IMapper mapper)
    {
        _damageReportRepository = damageReportRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<DamageReportDto>>> Handle(GetDamageReportsQuery request, CancellationToken cancellationToken)
    {
        var reports = await _damageReportRepository.GetAllAsync(
            request.WarehouseId,
            request.Status,
            request.DateFrom,
            request.DateTo);

        var dtos = _mapper.Map<IEnumerable<DamageReportDto>>(reports);
        return ApiResponse<IEnumerable<DamageReportDto>>.Ok(dtos);
    }
}
