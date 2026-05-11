using FluentValidation;

namespace CMS.IdentityService.Application.Commands.UpdateUser;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.Request.FirstName)
            .MaximumLength(100).WithMessage("FirstName must not exceed 100 characters.")
            .When(x => x.Request.FirstName != null);

        RuleFor(x => x.Request.LastName)
            .MaximumLength(100).WithMessage("LastName must not exceed 100 characters.")
            .When(x => x.Request.LastName != null);

        RuleFor(x => x.Request)
            .Must(r => r.FirstName != null || r.LastName != null || r.IsActive.HasValue)
            .WithMessage("At least one field (FirstName, LastName, or IsActive) must be provided for update.");
    }
}
