using AutoMapper;
using CMS.FleetService.Application.DTOs;
using CMS.FleetService.Domain.Enums;
using CMS.FleetService.Domain.Interfaces;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.FleetService.Application.Commands.UpdateVehicleStatus;

public class UpdateVehicleStatusCommandHandler : IRequestHandler<UpdateVehicleStatusCommand, ApiResponse<VehicleDto>>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IMapper _mapper;

    public UpdateVehicleStatusCommandHandler(IVehicleRepository vehicleRepository, IMapper mapper)
    {
        _vehicleRepository = vehicleRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<VehicleDto>> Handle(UpdateVehicleStatusCommand command, CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(command.VehicleId)
            ?? throw new NotFoundException("Vehicle", command.VehicleId);

        if (!Enum.TryParse<VehicleStatus>(command.Request.Status, ignoreCase: true, out var status))
            throw new ValidationException(new[] { $"Invalid status. Valid values: {string.Join(", ", Enum.GetNames<VehicleStatus>())}" });

        if (status == VehicleStatus.Available && vehicle.Status == VehicleStatus.InUse)
            throw new UnprocessableException("Cannot set an in-use vehicle to Available directly. Unassign the assignment first.");

        vehicle.SetStatus(status);
        await _vehicleRepository.UpdateAsync(vehicle);

        var dto = _mapper.Map<VehicleDto>(vehicle);
        return ApiResponse<VehicleDto>.Ok(dto, "Vehicle status updated.");
    }
}
