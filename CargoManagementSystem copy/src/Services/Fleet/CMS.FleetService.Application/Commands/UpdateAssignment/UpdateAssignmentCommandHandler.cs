using AutoMapper;
using CMS.FleetService.Application.DTOs;
using CMS.FleetService.Domain.Enums;
using CMS.FleetService.Domain.Interfaces;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.FleetService.Application.Commands.UpdateAssignment;

public class UpdateAssignmentCommandHandler : IRequestHandler<UpdateAssignmentCommand, ApiResponse<AssignmentDto>>
{
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IMapper _mapper;

    public UpdateAssignmentCommandHandler(
        IAssignmentRepository assignmentRepository,
        IVehicleRepository vehicleRepository,
        IMapper mapper)
    {
        _assignmentRepository = assignmentRepository;
        _vehicleRepository = vehicleRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<AssignmentDto>> Handle(UpdateAssignmentCommand command, CancellationToken cancellationToken)
    {
        var assignment = await _assignmentRepository.GetByIdAsync(command.AssignmentId)
            ?? throw new NotFoundException("Assignment", command.AssignmentId);

        if (assignment.Status != AssignmentStatus.Active)
            throw new UnprocessableException("Only active assignments can be updated.");

        if (command.Request.ScheduledPickup.HasValue)
            assignment.UpdateScheduledPickup(command.Request.ScheduledPickup.Value);

        if (command.Request.Notes != null)
            assignment.UpdateNotes(command.Request.Notes);

        // Vehicle swap
        if (command.Request.VehicleId.HasValue && command.Request.VehicleId.Value != assignment.VehicleId)
        {
            var oldVehicle = await _vehicleRepository.GetByIdAsync(assignment.VehicleId);
            var newVehicle = await _vehicleRepository.GetByIdAsync(command.Request.VehicleId.Value)
                ?? throw new NotFoundException("Vehicle", command.Request.VehicleId.Value);

            if (!newVehicle.CanBeAssigned())
                throw new UnprocessableException($"Vehicle '{command.Request.VehicleId}' is not available for assignment.");

            oldVehicle?.SetStatus(VehicleStatus.Available);
            newVehicle.SetStatus(VehicleStatus.InUse);

            if (oldVehicle != null) await _vehicleRepository.UpdateAsync(oldVehicle);
            await _vehicleRepository.UpdateAsync(newVehicle);

            assignment.SwapVehicle(command.Request.VehicleId.Value);
        }

        await _assignmentRepository.UpdateAsync(assignment);

        var dto = _mapper.Map<AssignmentDto>(assignment);
        return ApiResponse<AssignmentDto>.Ok(dto, "Assignment updated successfully.");
    }
}
