using FluentValidation;

namespace CMS.ShipmentService.Application.Commands.CreateShipment;

public class CreateShipmentCommandValidator : AbstractValidator<CreateShipmentCommand>
{
    public CreateShipmentCommandValidator()
    {
        RuleFor(x => x.Request.CustomerId)
            .NotEmpty().WithMessage("CustomerId is required.");

        RuleFor(x => x.Request.SenderName)
            .NotEmpty().WithMessage("SenderName is required.");

        RuleFor(x => x.Request.SenderContact)
            .NotEmpty().WithMessage("SenderContact is required.");

        RuleFor(x => x.Request.OriginAddress)
            .NotEmpty().WithMessage("OriginAddress is required.");

        RuleFor(x => x.Request.SenderCity)
            .NotEmpty().WithMessage("SenderCity is required.");

        RuleFor(x => x.Request.SenderCountry)
            .NotEmpty().WithMessage("SenderCountry is required.");

        RuleFor(x => x.Request.RecipientName)
            .NotEmpty().WithMessage("RecipientName is required.");

        RuleFor(x => x.Request.RecipientContact)
            .NotEmpty().WithMessage("RecipientContact is required.");

        RuleFor(x => x.Request.DestinationAddress)
            .NotEmpty().WithMessage("DestinationAddress is required.");

        RuleFor(x => x.Request.RecipientCity)
            .NotEmpty().WithMessage("RecipientCity is required.");

        RuleFor(x => x.Request.RecipientCountry)
            .NotEmpty().WithMessage("RecipientCountry is required.");

        RuleFor(x => x.Request.WeightKg)
            .GreaterThan(0).WithMessage("WeightKg must be greater than 0.");

        RuleFor(x => x.Request.VolumeCbm)
            .GreaterThan(0).WithMessage("VolumeCbm must be greater than 0.");

        RuleFor(x => x.Request.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be at least 1.");

        RuleFor(x => x.Request.CargoDescription)
            .NotEmpty().WithMessage("CargoDescription is required.");

        RuleFor(x => x.Request.ServiceType)
            .NotEmpty().WithMessage("ServiceType is required.");

        RuleFor(x => x.Request.CargoType)
            .NotEmpty().WithMessage("CargoType is required.");

        RuleFor(x => x.Request.PaymentMode)
            .NotEmpty().WithMessage("PaymentMode is required.");
    }
}
