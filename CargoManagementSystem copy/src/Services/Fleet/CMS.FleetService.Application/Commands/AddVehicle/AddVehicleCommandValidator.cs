using CMS.FleetService.Domain.Enums;
using FluentValidation;

namespace CMS.FleetService.Application.Commands.AddVehicle;

public class AddVehicleCommandValidator : AbstractValidator<AddVehicleCommand>
{
    public AddVehicleCommandValidator()
    {
        RuleFor(x => x.Request.PlateNumber).NotEmpty().WithMessage("Plate number is required.");
        RuleFor(x => x.Request.GpsDeviceId).NotEmpty().WithMessage("GPS device ID is required.");
        RuleFor(x => x.Request.Type)
            .NotEmpty().WithMessage("Vehicle type is required.")
            .Must(t => Enum.TryParse<VehicleType>(t, ignoreCase: true, out _))
            .WithMessage($"Vehicle type must be one of: {string.Join(", ", Enum.GetNames<VehicleType>())}.");
        RuleFor(x => x.Request.CapacityKg).GreaterThan(0).WithMessage("Capacity must be greater than 0.");
    }
}
