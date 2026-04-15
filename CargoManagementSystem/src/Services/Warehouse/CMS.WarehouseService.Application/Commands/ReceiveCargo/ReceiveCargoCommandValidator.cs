using FluentValidation;

namespace CMS.WarehouseService.Application.Commands.ReceiveCargo;

public class ReceiveCargoCommandValidator : AbstractValidator<ReceiveCargoCommand>
{
    public ReceiveCargoCommandValidator()
    {
        RuleFor(x => x.Request.ShipmentId)
            .NotEmpty().WithMessage("ShipmentId is required.");

        RuleFor(x => x.Request.WarehouseId)
            .NotEmpty().WithMessage("WarehouseId is required.");
    }
}
