using AutoMapper;
using CMS.FleetService.Application.DTOs;
using CMS.FleetService.Domain.Entities;
using CMS.FleetService.Domain.Enums;
using CMS.FleetService.Domain.Interfaces;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.FleetService.Application.Commands.AddMaintenanceLog;

public class AddMaintenanceLogCommandHandler : IRequestHandler<AddMaintenanceLogCommand, ApiResponse<MaintenanceLogDto>>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IMaintenanceLogRepository _maintenanceLogRepository;
    private readonly IMapper _mapper;

    public AddMaintenanceLogCommandHandler(
        IVehicleRepository vehicleRepository,
        IMaintenanceLogRepository maintenanceLogRepository,
        IMapper mapper)
    {
        _vehicleRepository = vehicleRepository;
        _maintenanceLogRepository = maintenanceLogRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<MaintenanceLogDto>> Handle(AddMaintenanceLogCommand command, CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(command.VehicleId)
            ?? throw new NotFoundException("Vehicle", command.VehicleId);

        var log = MaintenanceLog.Create(
            command.VehicleId,
            command.Request.MaintenanceType,
            command.Request.ServiceDate,
            command.Request.MileageAtService,
            command.Request.NextServiceDate,
            command.Request.NextServiceMileage,
            command.Request.ServiceProvider,
            command.Request.Notes,
            command.Request.Cost);

        await _maintenanceLogRepository.AddAsync(log);

        // Set vehicle to Maintenance status
        vehicle.SetStatus(VehicleStatus.Maintenance);
        if (command.Request.NextServiceDate.HasValue)
            vehicle.AddMaintenanceLog(log);

        await _vehicleRepository.UpdateAsync(vehicle);

        var dto = _mapper.Map<MaintenanceLogDto>(log);
        return ApiResponse<MaintenanceLogDto>.Ok(dto, "Maintenance log added. Vehicle set to Maintenance status.");
    }
}
