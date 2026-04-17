using AutoMapper;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using CMS.WarehouseService.Application.DTOs;
using CMS.WarehouseService.Domain.Entities;
using CMS.WarehouseService.Domain.Interfaces;
using MediatR;

namespace CMS.WarehouseService.Application.Commands.UpdateDamageReport;

public class UpdateDamageReportCommandHandler : IRequestHandler<UpdateDamageReportCommand, ApiResponse<DamageReportDto>>
{
    private readonly IDamageReportRepository _damageReportRepository;
    private readonly IMapper _mapper;

    public UpdateDamageReportCommandHandler(IDamageReportRepository damageReportRepository, IMapper mapper)
    {
        _damageReportRepository = damageReportRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<DamageReportDto>> Handle(UpdateDamageReportCommand command, CancellationToken cancellationToken)
    {
        var report = await _damageReportRepository.GetByIdAsync(command.ReportId)
            ?? throw new NotFoundException("DamageReport", command.ReportId);

        if (report.Status == DamageReportStatus.Resolved)
            throw new UnprocessableException("Cannot update a damage report that is already resolved.");

        var newStatus = command.Request.Status.ToLowerInvariant() switch
        {
            "acknowledged" => DamageReportStatus.Acknowledged,
            "resolved" => DamageReportStatus.Resolved,
            _ => throw new ValidationException($"Invalid status '{command.Request.Status}'. Valid values: Acknowledged, Resolved.")
        };

        if (newStatus == DamageReportStatus.Acknowledged)
            report.Acknowledge();
        else
            report.Resolve(command.Request.ResolutionNotes ?? string.Empty);

        await _damageReportRepository.UpdateAsync(report);

        var dto = _mapper.Map<DamageReportDto>(report);
        return ApiResponse<DamageReportDto>.Ok(dto, "Damage report updated successfully.");
    }
}
