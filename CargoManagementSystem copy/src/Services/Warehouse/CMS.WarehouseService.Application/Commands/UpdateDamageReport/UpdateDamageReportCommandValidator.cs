using FluentValidation;

namespace CMS.WarehouseService.Application.Commands.UpdateDamageReport;

public class UpdateDamageReportCommandValidator : AbstractValidator<UpdateDamageReportCommand>
{
    private static readonly string[] ValidStatuses = { "Acknowledged", "Resolved" };

    public UpdateDamageReportCommandValidator()
    {
        RuleFor(x => x.ReportId)
            .NotEmpty().WithMessage("ReportId is required.");

        RuleFor(x => x.Request.Status)
            .NotEmpty().WithMessage("Status is required.")
            .Must(s => ValidStatuses.Contains(s, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Status must be 'Acknowledged' or 'Resolved'.");

        RuleFor(x => x.Request.ResolutionNotes)
            .NotEmpty().WithMessage("ResolutionNotes are required when resolving a damage report.")
            .When(x => x.Request.Status.Equals("Resolved", StringComparison.OrdinalIgnoreCase));

        RuleFor(x => x.Request.ResolutionNotes)
            .MaximumLength(2000).WithMessage("ResolutionNotes must not exceed 2000 characters.")
            .When(x => x.Request.ResolutionNotes != null);
    }
}
