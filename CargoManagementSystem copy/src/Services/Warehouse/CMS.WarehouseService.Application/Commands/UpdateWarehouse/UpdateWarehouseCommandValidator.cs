using FluentValidation;

namespace CMS.WarehouseService.Application.Commands.UpdateWarehouse;

public class UpdateWarehouseCommandValidator : AbstractValidator<UpdateWarehouseCommand>
{
    public UpdateWarehouseCommandValidator()
    {
        RuleFor(x => x.WarehouseId)
            .NotEmpty().WithMessage("WarehouseId is required.");

        RuleFor(x => x.Request.Name)
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.")
            .When(x => x.Request.Name != null);

        RuleFor(x => x.Request.Address)
            .MaximumLength(500).WithMessage("Address must not exceed 500 characters.")
            .When(x => x.Request.Address != null);

        RuleFor(x => x.Request.City)
            .MaximumLength(100).WithMessage("City must not exceed 100 characters.")
            .When(x => x.Request.City != null);

        RuleFor(x => x.Request.Country)
            .MaximumLength(100).WithMessage("Country must not exceed 100 characters.")
            .When(x => x.Request.Country != null);

        RuleFor(x => x.Request.CapacityKg)
            .GreaterThan(0).WithMessage("CapacityKg must be greater than 0.")
            .When(x => x.Request.CapacityKg.HasValue);
    }
}
