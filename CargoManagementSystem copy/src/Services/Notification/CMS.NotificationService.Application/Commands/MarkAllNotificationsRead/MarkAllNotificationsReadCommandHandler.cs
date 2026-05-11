using CMS.NotificationService.Domain.Interfaces;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.NotificationService.Application.Commands.MarkAllNotificationsRead;

public class MarkAllNotificationsReadCommandHandler : IRequestHandler<MarkAllNotificationsReadCommand, ApiResponse<int>>
{
    private readonly INotificationRepository _repository;

    public MarkAllNotificationsReadCommandHandler(INotificationRepository repository)
    {
        _repository = repository;
    }

    public async Task<ApiResponse<int>> Handle(MarkAllNotificationsReadCommand command, CancellationToken cancellationToken)
    {
        var unread = await _repository.GetUnreadByRecipientAsync(command.RecipientId);
        var count = 0;

        foreach (var record in unread)
        {
            record.MarkAsRead();
            await _repository.UpdateAsync(record);
            count++;
        }

        return ApiResponse<int>.Ok(count, $"{count} notification(s) marked as read.");
    }
}
