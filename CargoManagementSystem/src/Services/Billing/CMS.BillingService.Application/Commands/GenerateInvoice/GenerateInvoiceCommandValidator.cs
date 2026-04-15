using FluentValidation;

namespace CMS.BillingService.Application.Commands.GenerateInvoice;

public class GenerateInvoiceCommandValidator : AbstractValidator<GenerateInvoiceCommand>
{
    public GenerateInvoiceCommandValidator()
    {
        RuleFor(x => x.Request.ShipmentId)
            .NotEmpty().WithMessage("ShipmentId is required.");

        RuleFor(x => x.Request.CustomerId)
            .NotEmpty().WithMessage("CustomerId is required.");

        RuleFor(x => x.Request.BaseFreightCharge)
            .GreaterThan(0).WithMessage("BaseFreightCharge must be greater than 0.");

        RuleFor(x => x.Request.TaxRate)
            .InclusiveBetween(0, 1).WithMessage("TaxRate must be between 0 and 1 (e.g. 0.18 for 18%).");
    }
}
