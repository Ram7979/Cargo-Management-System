using FluentValidation;

namespace CMS.ShipmentService.Application.Commands.CreateShipment;

public class CreateShipmentCommandValidator : AbstractValidator<CreateShipmentCommand>
{
    public CreateShipmentCommandValidator()
    {
        RuleFor(x => x.Request.CustomerId)
            .NotEmpty().WithMessage("CustomerId is required.");

        RuleFor(x => x.Request.OriginAddress)
            .NotEmpty().WithMessage("OriginAddress is required.");

        RuleFor(x => x.Request.DestinationAddress)
            .NotEmpty().WithMessage("DestinationAddress is required.");

        RuleFor(x => x.Request.WeightKg)
            .GreaterThan(0).WithMessage("WeightKg must be greater than 0.");

        RuleFor(x => x.Request.CargoDescription)
            .NotEmpty().WithMessage("CargoDescription is required.");

        RuleFor(x => x.Request.ServiceType)
            .NotEmpty().WithMessage("ServiceType is required.");
    }
}
