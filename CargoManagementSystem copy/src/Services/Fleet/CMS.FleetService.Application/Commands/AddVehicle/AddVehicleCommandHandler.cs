using AutoMapper;
using CMS.FleetService.Application.DTOs;
using CMS.FleetService.Domain.Entities;
using CMS.FleetService.Domain.Enums;
using CMS.FleetService.Domain.Interfaces;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.FleetService.Application.Commands.AddVehicle;

public class AddVehicleCommandHandler : IRequestHandler<AddVehicleCommand, ApiResponse<VehicleDto>>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IMapper _mapper;

    public AddVehicleCommandHandler(IVehicleRepository vehicleRepository, IMapper mapper)
    {
        _vehicleRepository = vehicleRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<VehicleDto>> Handle(AddVehicleCommand command, CancellationToken cancellationToken)
    {
        // Check plate number uniqueness at application layer
        var existing = await _vehicleRepository.GetByPlateNumberAsync(command.Request.PlateNumber);
        if (existing != null)
            throw new ConflictException($"Vehicle with plate number '{command.Request.PlateNumber}' already exists.");

        if (!Enum.TryParse<VehicleType>(command.Request.Type, ignoreCase: true, out var vehicleType))
            throw new ValidationException(new[] { $"Invalid vehicle type. Valid values: {string.Join(", ", Enum.GetNames<VehicleType>())}" });

        if (!Enum.TryParse<FuelType>(command.Request.FuelType, ignoreCase: true, out var fuelType))
            throw new ValidationException(new[] { $"Invalid fuel type. Valid values: {string.Join(", ", Enum.GetNames<FuelType>())}" });

        var vehicle = Vehicle.Create(
            command.Request.PlateNumber,
            command.Request.GpsDeviceId,
            vehicleType,
            command.Request.CapacityKg,
            command.Request.Make,
            command.Request.Model,
            command.Request.Year,
            fuelType,
            command.Request.VolumeCbm,
            command.Request.RegistrationCertUrl,
            command.Request.InsuranceDocUrl,
            command.Request.NextServiceDate);

        await _vehicleRepository.AddAsync(vehicle);

        var dto = _mapper.Map<VehicleDto>(vehicle);
        return ApiResponse<VehicleDto>.Ok(dto, "Vehicle added successfully.");
    }
}
