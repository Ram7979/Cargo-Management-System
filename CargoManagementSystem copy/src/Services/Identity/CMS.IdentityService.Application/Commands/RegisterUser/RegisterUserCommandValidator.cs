using FluentValidation;

namespace CMS.IdentityService.Application.Commands.RegisterUser;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Request.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be a valid email address.");

        RuleFor(x => x.Request.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.");

        RuleFor(x => x.Request.FirstName)
            .NotEmpty().WithMessage("First name is required.");

        RuleFor(x => x.Request.LastName)
            .NotEmpty().WithMessage("Last name is required.");

        RuleFor(x => x.Request.Role)
            .NotEmpty().WithMessage("Role is required.");
    }
}
