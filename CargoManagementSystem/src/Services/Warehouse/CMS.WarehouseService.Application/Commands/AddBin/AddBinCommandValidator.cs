using FluentValidation;

namespace CMS.WarehouseService.Application.Commands.AddBin;

public class AddBinCommandValidator : AbstractValidator<AddBinCommand>
{
    public AddBinCommandValidator()
    {
        RuleFor(x => x.WarehouseId)
            .NotEmpty().WithMessage("WarehouseId is required.");

        RuleFor(x => x.Request.BinCode)
            .NotEmpty().WithMessage("BinCode is required.")
            .MaximumLength(50).WithMessage("BinCode must not exceed 50 characters.");

        RuleFor(x => x.Request.CapacityKg)
            .GreaterThan(0).WithMessage("CapacityKg must be greater than 0.");

        RuleFor(x => x.Request.Zone)
            .MaximumLength(100).WithMessage("Zone must not exceed 100 characters.")
            .When(x => x.Request.Zone != null);

        RuleFor(x => x.Request.Level)
            .MaximumLength(50).WithMessage("Level must not exceed 50 characters.")
            .When(x => x.Request.Level != null);
    }
}
