using FluentValidation;

namespace CMS.FleetService.Application.Commands.CreateAssignment;

public class CreateAssignmentCommandValidator : AbstractValidator<CreateAssignmentCommand>
{
    public CreateAssignmentCommandValidator()
    {
        RuleFor(x => x.Request.ShipmentId).NotEmpty().WithMessage("Shipment ID is required.");
        RuleFor(x => x.Request.DriverId).NotEmpty().WithMessage("Driver ID is required.");
        RuleFor(x => x.Request.VehicleId).NotEmpty().WithMessage("Vehicle ID is required.");
    }
}
