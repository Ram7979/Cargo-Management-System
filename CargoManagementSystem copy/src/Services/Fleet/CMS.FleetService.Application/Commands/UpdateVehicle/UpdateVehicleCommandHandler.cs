using AutoMapper;
using CMS.FleetService.Application.DTOs;
using CMS.FleetService.Domain.Enums;
using CMS.FleetService.Domain.Interfaces;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.FleetService.Application.Commands.UpdateVehicle;

public class UpdateVehicleCommandHandler : IRequestHandler<UpdateVehicleCommand, ApiResponse<VehicleDto>>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IMapper _mapper;

    public UpdateVehicleCommandHandler(IVehicleRepository vehicleRepository, IMapper mapper)
    {
        _vehicleRepository = vehicleRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<VehicleDto>> Handle(UpdateVehicleCommand command, CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(command.VehicleId)
            ?? throw new NotFoundException("Vehicle", command.VehicleId);

        FuelType? fuelType = null;
        if (!string.IsNullOrWhiteSpace(command.Request.FuelType))
        {
            if (!Enum.TryParse<FuelType>(command.Request.FuelType, ignoreCase: true, out var ft))
                throw new ValidationException(new[] { $"Invalid FuelType. Valid values: {string.Join(", ", Enum.GetNames<FuelType>())}" });
            fuelType = ft;
        }

        vehicle.UpdateDetails(command.Request.Make, command.Request.Model, command.Request.Year,
            fuelType, command.Request.CapacityKg, command.Request.VolumeCbm,
            command.Request.GpsDeviceId, command.Request.InsuranceDocUrl);

        await _vehicleRepository.UpdateAsync(vehicle);

        var dto = _mapper.Map<VehicleDto>(vehicle);
        return ApiResponse<VehicleDto>.Ok(dto, "Vehicle updated successfully.");
    }
}
