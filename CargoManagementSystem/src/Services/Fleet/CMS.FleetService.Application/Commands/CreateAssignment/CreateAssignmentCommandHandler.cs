using AutoMapper;
using CMS.FleetService.Application.DTOs;
using CMS.FleetService.Application.Events;
using CMS.FleetService.Domain.Entities;
using CMS.FleetService.Domain.Enums;
using CMS.FleetService.Domain.Exceptions;
using CMS.FleetService.Domain.Interfaces;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.FleetService.Application.Commands.CreateAssignment;

public class CreateAssignmentCommandHandler : IRequestHandler<CreateAssignmentCommand, ApiResponse<AssignmentDto>>
{
    private readonly IDriverRepository _driverRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public CreateAssignmentCommandHandler(
        IDriverRepository driverRepository,
        IVehicleRepository vehicleRepository,
        IAssignmentRepository assignmentRepository,
        IMediator mediator,
        IMapper mapper)
    {
        _driverRepository = driverRepository;
        _vehicleRepository = vehicleRepository;
        _assignmentRepository = assignmentRepository;
        _mediator = mediator;
        _mapper = mapper;
    }

    public async Task<ApiResponse<AssignmentDto>> Handle(CreateAssignmentCommand command, CancellationToken cancellationToken)
    {
        var driver = await _driverRepository.GetByIdAsync(command.Request.DriverId)
            ?? throw new DriverNotFoundException($"Driver '{command.Request.DriverId}' not found.");

        // Driver availability check — soft warning with forceAssign override
        if (driver.Status == DriverStatus.OnDuty && !command.Request.ForceAssign)
            throw new DriverNotAvailableException(
                $"Driver '{command.Request.DriverId}' is currently on duty. Set forceAssign=true to override.");

        if (driver.Status == DriverStatus.OffDuty)
            throw new DriverNotAvailableException($"Driver '{command.Request.DriverId}' is off duty.");

        if (driver.IsLicenseExpired())
            throw new DriverNotAvailableException($"Driver '{command.Request.DriverId}' has an expired license.");

        var vehicle = await _vehicleRepository.GetByIdAsync(command.Request.VehicleId)
            ?? throw new VehicleNotFoundException($"Vehicle '{command.Request.VehicleId}' not found.");

        if (!vehicle.CanBeAssigned())
            throw new InsufficientCapacityException($"Vehicle '{command.Request.VehicleId}' is not available (status: {vehicle.Status}).");

        // Real capacity check using shipment weight
        var shipmentWeight = command.Request.ShipmentWeightKg;
        if (shipmentWeight > 0 && !vehicle.HasCapacityFor(shipmentWeight))
            throw new InsufficientCapacityException(
                $"Vehicle capacity exceeded. Available: {vehicle.CapacityKg - vehicle.CurrentLoadKg}kg, Required: {shipmentWeight}kg.");

        var assignment = Assignment.Create(
            command.Request.ShipmentId,
            command.Request.DriverId,
            command.Request.VehicleId,
            command.Request.ScheduledPickup,
            command.Request.Notes ?? string.Empty);

        driver.SetStatus(DriverStatus.OnDuty);
        vehicle.SetStatus(VehicleStatus.InUse);
        if (shipmentWeight > 0) vehicle.AddLoad(shipmentWeight);

        await _assignmentRepository.AddAsync(assignment);
        await _driverRepository.UpdateAsync(driver);
        await _vehicleRepository.UpdateAsync(vehicle);

        await _mediator.Publish(
            new ShipmentAssignedEvent(
                command.Request.ShipmentId,
                command.Request.DriverId,
                command.Request.VehicleId,
                assignment.Id),
            cancellationToken);

        var dto = _mapper.Map<AssignmentDto>(assignment);
        return ApiResponse<AssignmentDto>.Ok(dto, "Assignment created successfully.");
    }
}
