using FluentValidation;

namespace CMS.WarehouseService.Application.Commands.ReceiveCargo;

public class ReceiveCargoCommandValidator : AbstractValidator<ReceiveCargoCommand>
{
    public ReceiveCargoCommandValidator()
    {
        RuleFor(x => x.Request.TrackingNumber)
            .NotEmpty().WithMessage("TrackingNumber is required.")
            .Matches(@"^CMS-\d{4}-\d+$").WithMessage("TrackingNumber must be in format CMS-YYYY-XXXXXX.");

        RuleFor(x => x.Request.WarehouseId)
            .NotEmpty().WithMessage("WarehouseId is required.");

        RuleFor(x => x.Request.DamageNotes)
            .NotEmpty().WithMessage("DamageNotes are required when HasDamageReport is true.")
            .When(x => x.Request.HasDamageReport);

        RuleFor(x => x.Request.DamageNotes)
            .MaximumLength(2000).WithMessage("DamageNotes must not exceed 2000 characters.")
            .When(x => x.Request.DamageNotes != null);

        RuleFor(x => x.Request.Remarks)
            .MaximumLength(1000).WithMessage("Remarks must not exceed 1000 characters.")
            .When(x => x.Request.Remarks != null);
    }
}
