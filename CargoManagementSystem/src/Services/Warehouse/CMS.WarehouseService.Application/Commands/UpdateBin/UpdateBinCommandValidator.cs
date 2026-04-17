using FluentValidation;

namespace CMS.WarehouseService.Application.Commands.UpdateBin;

public class UpdateBinCommandValidator : AbstractValidator<UpdateBinCommand>
{
    public UpdateBinCommandValidator()
    {
        RuleFor(x => x.WarehouseId)
            .NotEmpty().WithMessage("WarehouseId is required.");

        RuleFor(x => x.BinId)
            .NotEmpty().WithMessage("BinId is required.");

        RuleFor(x => x.Request.BinCode)
            .MaximumLength(50).WithMessage("BinCode must not exceed 50 characters.")
            .When(x => x.Request.BinCode != null);

        RuleFor(x => x.Request.CapacityKg)
            .GreaterThan(0).WithMessage("CapacityKg must be greater than 0.")
            .When(x => x.Request.CapacityKg.HasValue);

        RuleFor(x => x.Request.Zone)
            .MaximumLength(100).WithMessage("Zone must not exceed 100 characters.")
            .When(x => x.Request.Zone != null);

        RuleFor(x => x.Request.Level)
            .MaximumLength(50).WithMessage("Level must not exceed 50 characters.")
            .When(x => x.Request.Level != null);
    }
}
