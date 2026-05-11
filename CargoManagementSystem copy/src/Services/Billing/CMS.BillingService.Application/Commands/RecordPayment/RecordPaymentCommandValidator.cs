using CMS.BillingService.Domain.Enums;
using FluentValidation;

namespace CMS.BillingService.Application.Commands.RecordPayment;

public class RecordPaymentCommandValidator : AbstractValidator<RecordPaymentCommand>
{
    public RecordPaymentCommandValidator()
    {
        RuleFor(x => x.Request.InvoiceId)
            .NotEmpty().WithMessage("InvoiceId is required.");

        RuleFor(x => x.Request.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than 0.");

        RuleFor(x => x.Request.Method)
            .NotEmpty().WithMessage("Method is required.")
            .Must(m => Enum.TryParse<PaymentMethod>(m, ignoreCase: true, out _))
            .WithMessage($"Method must be one of: {string.Join(", ", Enum.GetNames<PaymentMethod>())}.");
    }
}
