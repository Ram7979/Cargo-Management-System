using CMS.NotificationService.Domain.Enums;
using FluentValidation;

namespace CMS.NotificationService.Application.Commands.QueueNotification;

public class QueueNotificationCommandValidator : AbstractValidator<QueueNotificationCommand>
{
    public QueueNotificationCommandValidator()
    {
        RuleFor(x => x.Request.RecipientId)
            .NotEmpty().WithMessage("RecipientId is required.");

        RuleFor(x => x.Request.Channel)
            .NotEmpty().WithMessage("Channel is required.")
            .Must(c => Enum.TryParse<NotificationChannel>(c, ignoreCase: true, out _))
            .WithMessage("Channel must be 'Email', 'SMS', or 'Push'.");

        RuleFor(x => x.Request.Recipient)
            .NotEmpty().WithMessage("Recipient is required.");

        RuleFor(x => x.Request.Subject)
            .NotEmpty().WithMessage("Subject is required.")
            .MaximumLength(512).WithMessage("Subject must not exceed 512 characters.");

        RuleFor(x => x.Request.Body)
            .NotEmpty().WithMessage("Body is required.");
    }
}
