using FluentValidation;

namespace CMS.WarehouseService.Application.Commands.ReleaseCargo;

public class ReleaseCargoCommandValidator : AbstractValidator<ReleaseCargoCommand>
{
    public ReleaseCargoCommandValidator()
    {
        RuleFor(x => x.Request.ShipmentId)
            .NotEmpty().WithMessage("ShipmentId is required.");

        RuleFor(x => x.Request.WarehouseId)
            .NotEmpty().WithMessage("WarehouseId is required.");

        RuleFor(x => x.Request.Notes)
            .MaximumLength(1000).WithMessage("Notes must not exceed 1000 characters.")
            .When(x => x.Request.Notes != null);
    }
}
