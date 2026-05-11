using CMS.NotificationService.Domain.Interfaces;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.NotificationService.Application.Commands.DeleteNotification;

public class DeleteNotificationCommandHandler : IRequestHandler<DeleteNotificationCommand, ApiResponse<bool>>
{
    private readonly INotificationRepository _repository;

    public DeleteNotificationCommandHandler(INotificationRepository repository)
    {
        _repository = repository;
    }

    public async Task<ApiResponse<bool>> Handle(DeleteNotificationCommand command, CancellationToken cancellationToken)
    {
        var record = await _repository.GetByIdAsync(command.NotificationId)
            ?? throw new NotFoundException("Notification", command.NotificationId);

        record.SoftDelete();
        await _repository.UpdateAsync(record);

        return ApiResponse<bool>.Ok(true, "Notification deleted.");
    }
}
