using FluentValidation;

namespace CMS.BillingService.Application.Commands.UpdateRateCard;

public class UpdateRateCardCommandValidator : AbstractValidator<UpdateRateCardCommand>
{
    public UpdateRateCardCommandValidator()
    {
        RuleFor(x => x.RateCardId)
            .NotEmpty().WithMessage("RateCardId is required.");

        RuleFor(x => x.Request.BaseRatePerKg)
            .GreaterThan(0).WithMessage("BaseRatePerKg must be greater than 0.");

        RuleFor(x => x.Request.FuelSurchargePercent)
            .GreaterThanOrEqualTo(0).WithMessage("FuelSurchargePercent must be 0 or greater.")
            .LessThanOrEqualTo(100).WithMessage("FuelSurchargePercent must not exceed 100.");

        RuleFor(x => x.Request.HandlingFeeFlat)
            .GreaterThanOrEqualTo(0).WithMessage("HandlingFeeFlat must be 0 or greater.");

        RuleFor(x => x.Request.TaxPercent)
            .GreaterThanOrEqualTo(0).WithMessage("TaxPercent must be 0 or greater.")
            .LessThanOrEqualTo(100).WithMessage("TaxPercent must not exceed 100.");
    }
}
