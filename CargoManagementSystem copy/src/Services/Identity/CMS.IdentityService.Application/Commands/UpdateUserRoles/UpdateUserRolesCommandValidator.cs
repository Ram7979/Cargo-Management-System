using FluentValidation;

namespace CMS.IdentityService.Application.Commands.UpdateUserRoles;

public class UpdateUserRolesCommandValidator : AbstractValidator<UpdateUserRolesCommand>
{
    public UpdateUserRolesCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.Request.Roles)
            .NotEmpty().WithMessage("At least one role must be specified.")
            .Must(roles => roles != null && roles.Any())
            .WithMessage("Roles collection must not be empty.");
    }
}
