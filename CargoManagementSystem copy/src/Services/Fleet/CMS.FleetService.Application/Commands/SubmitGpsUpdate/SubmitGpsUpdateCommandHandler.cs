using CMS.FleetService.Application.Interfaces;
using CMS.FleetService.Domain.Entities;
using CMS.FleetService.Domain.Exceptions;
using CMS.FleetService.Domain.Interfaces;
using CMS.FleetService.Domain.ValueObjects;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.FleetService.Application.Commands.SubmitGpsUpdate;

public class SubmitGpsUpdateCommandHandler : IRequestHandler<SubmitGpsUpdateCommand, ApiResponse<bool>>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly IGpsHistoryRepository _gpsHistoryRepository;
    private readonly ICacheService _cacheService;

    public SubmitGpsUpdateCommandHandler(
        IVehicleRepository vehicleRepository,
        IAssignmentRepository assignmentRepository,
        IGpsHistoryRepository gpsHistoryRepository,
        ICacheService cacheService)
    {
        _vehicleRepository = vehicleRepository;
        _assignmentRepository = assignmentRepository;
        _gpsHistoryRepository = gpsHistoryRepository;
        _cacheService = cacheService;
    }

    public async Task<ApiResponse<bool>> Handle(SubmitGpsUpdateCommand command, CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(command.VehicleId)
            ?? throw new VehicleNotFoundException($"Vehicle '{command.VehicleId}' not found.");

        var activeAssignment = await _assignmentRepository.GetActiveByVehicleIdAsync(command.VehicleId);
        if (activeAssignment == null || activeAssignment.DriverId != command.DriverId)
            throw new ForbiddenException("You are not assigned to this vehicle.");

        var now = DateTime.UtcNow;
        var location = new GpsCoordinate(command.Request.Latitude, command.Request.Longitude, now);
        vehicle.UpdateLocation(location);
        await _vehicleRepository.UpdateAsync(vehicle);

        // Persist GPS breadcrumb trail
        var history = GpsHistory.Create(command.VehicleId, activeAssignment.Id,
            command.Request.Latitude, command.Request.Longitude, now);
        await _gpsHistoryRepository.AddAsync(history);

        // Cache for live map
        var cacheKey = $"fleet:live:{command.VehicleId}";
        await _cacheService.SetAsync(cacheKey, location, TimeSpan.FromSeconds(30));

        return ApiResponse<bool>.Ok(true, "GPS location updated.");
    }
}
