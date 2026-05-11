using CMS.NotificationService.Domain.Interfaces;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.NotificationService.Application.Commands.MarkNotificationRead;

public class MarkNotificationReadCommandHandler : IRequestHandler<MarkNotificationReadCommand, ApiResponse<bool>>
{
    private readonly INotificationRepository _repository;

    public MarkNotificationReadCommandHandler(INotificationRepository repository)
    {
        _repository = repository;
    }

    public async Task<ApiResponse<bool>> Handle(MarkNotificationReadCommand command, CancellationToken cancellationToken)
    {
        var record = await _repository.GetByIdAsync(command.NotificationId)
            ?? throw new NotFoundException("Notification", command.NotificationId);

        record.MarkAsRead();
        await _repository.UpdateAsync(record);

        return ApiResponse<bool>.Ok(true, "Notification marked as read.");
    }
}
