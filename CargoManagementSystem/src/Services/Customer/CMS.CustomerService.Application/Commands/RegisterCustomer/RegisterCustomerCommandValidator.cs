using CMS.CustomerService.Domain.Enums;
using FluentValidation;

namespace CMS.CustomerService.Application.Commands.RegisterCustomer;

public class RegisterCustomerCommandValidator : AbstractValidator<RegisterCustomerCommand>
{
    public RegisterCustomerCommandValidator()
    {
        RuleFor(x => x.Request.FullName).NotEmpty().WithMessage("Full name is required.");
        RuleFor(x => x.Request.Email).NotEmpty().EmailAddress().WithMessage("A valid email is required.");
        RuleFor(x => x.Request.Phone).NotEmpty().WithMessage("Phone is required.");
        RuleFor(x => x.Request.Address).NotEmpty().WithMessage("Address is required.");
        RuleFor(x => x.Request.City).NotEmpty().WithMessage("City is required.");
        RuleFor(x => x.Request.Country).NotEmpty().WithMessage("Country is required.");
        RuleFor(x => x.Request.Type)
            .NotEmpty().WithMessage("Customer type is required.")
            .Must(t => Enum.TryParse<CustomerType>(t, ignoreCase: true, out _))
            .WithMessage($"Type must be one of: {string.Join(", ", Enum.GetNames<CustomerType>())}.");
    }
}
