using NovaScale.Platform.API.ConversationManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.Commands;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.Queries;

namespace NovaScale.Platform.API.ConversationManagement.Application.Services;

public interface INotificationCommandService
{
    Task<Notification> Handle(CreateNotificationCommand command, CancellationToken cancellationToken = default);
    Task<Notification> Handle(long id, UpdateNotificationCommand command, CancellationToken cancellationToken = default);
    Task RemoveAsync(long id, CancellationToken cancellationToken = default);
}
