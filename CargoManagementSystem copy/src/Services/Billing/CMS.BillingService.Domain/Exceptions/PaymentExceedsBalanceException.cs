namespace CMS.BillingService.Domain.Exceptions;

public class PaymentExceedsBalanceException : Exception
{
    public PaymentExceedsBalanceException(decimal amount, decimal balance)
        : base($"Payment amount {amount:C} exceeds outstanding balance {balance:C}.") { }

    public PaymentExceedsBalanceException(string message) : base(message) { }
}
