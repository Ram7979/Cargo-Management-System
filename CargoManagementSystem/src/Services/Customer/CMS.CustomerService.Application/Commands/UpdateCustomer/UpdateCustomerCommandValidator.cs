using FluentValidation;

namespace CMS.CustomerService.Application.Commands.UpdateCustomer;

public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerCommandValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty().WithMessage("Customer ID is required.");
        RuleFor(x => x.Request.FullName).NotEmpty().WithMessage("Full name is required.");
    }
}
