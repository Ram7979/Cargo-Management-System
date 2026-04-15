using CMS.FleetService.Application.Interfaces;
using CMS.FleetService.Domain.Enums;
using CMS.FleetService.Domain.Interfaces;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CMS.FleetService.Application.Commands.UnassignAssignment;

public class UnassignAssignmentCommandHandler : IRequestHandler<UnassignAssignmentCommand, ApiResponse<bool>>
{
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly IDriverRepository _driverRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IShipmentServiceClient _shipmentServiceClient;
    private readonly ILogger<UnassignAssignmentCommandHandler> _logger;

    public UnassignAssignmentCommandHandler(
        IAssignmentRepository assignmentRepository,
        IDriverRepository driverRepository,
        IVehicleRepository vehicleRepository,
        IShipmentServiceClient shipmentServiceClient,
        ILogger<UnassignAssignmentCommandHandler> logger)
    {
        _assignmentRepository = assignmentRepository;
        _driverRepository = driverRepository;
        _vehicleRepository = vehicleRepository;
        _shipmentServiceClient = shipmentServiceClient;
        _logger = logger;
    }

    public async Task<ApiResponse<bool>> Handle(UnassignAssignmentCommand command, CancellationToken cancellationToken)
    {
        var assignment = await _assignmentRepository.GetByIdAsync(command.AssignmentId)
            ?? throw new NotFoundException("Assignment", command.AssignmentId);

        if (assignment.Status != AssignmentStatus.Active)
            throw new UnprocessableException("Only active assignments can be unassigned.");

        assignment.Cancel(command.Request.Reason);

        // Free driver and vehicle
        var driver = await _driverRepository.GetByIdAsync(assignment.DriverId);
        var vehicle = await _vehicleRepository.GetByIdAsync(assignment.VehicleId);

        driver?.SetStatus(DriverStatus.Available);
        vehicle?.SetStatus(VehicleStatus.Available);
        vehicle?.RemoveLoad(0); // Reset load

        await _assignmentRepository.UpdateAsync(assignment);
        if (driver != null) await _driverRepository.UpdateAsync(driver);
        if (vehicle != null) await _vehicleRepository.UpdateAsync(vehicle);

        // Notify Shipment Service to revert to Pending
        try
        {
            await _shipmentServiceClient.UpdateShipmentStatusAsync(assignment.ShipmentId, "Pending", "FleetService");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to revert shipment {ShipmentId} to Pending after unassign.", assignment.ShipmentId);
        }

        return ApiResponse<bool>.Ok(true, "Assignment cancelled. Driver and vehicle are now available.");
    }
}
