using FluentValidation;

namespace CMS.FleetService.Application.Commands.SubmitGpsUpdate;

public class SubmitGpsUpdateCommandValidator : AbstractValidator<SubmitGpsUpdateCommand>
{
    public SubmitGpsUpdateCommandValidator()
    {
        RuleFor(x => x.VehicleId).NotEmpty().WithMessage("Vehicle ID is required.");
        RuleFor(x => x.Request.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90.");
        RuleFor(x => x.Request.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180.");
    }
}
