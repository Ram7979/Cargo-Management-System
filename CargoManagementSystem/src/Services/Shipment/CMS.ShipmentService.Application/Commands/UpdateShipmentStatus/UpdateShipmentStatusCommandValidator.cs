using FluentValidation;

namespace CMS.ShipmentService.Application.Commands.UpdateShipmentStatus;

public class UpdateShipmentStatusCommandValidator : AbstractValidator<UpdateShipmentStatusCommand>
{
    public UpdateShipmentStatusCommandValidator()
    {
        RuleFor(x => x.ShipmentId)
            .NotEmpty().WithMessage("ShipmentId is required.");

        RuleFor(x => x.Request.Status)
            .NotEmpty().WithMessage("Status is required.");
    }
}
