using NovaScale.Platform.API.ConversationManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.Commands;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.Queries;
using NovaScale.Platform.API.Shared.Application.Patterns;

namespace NovaScale.Platform.API.ConversationManagement.Application.Services;

public interface INotificationQueryService
{
    Task<Notification> Handle(GetNotificationByIdQuery query, CancellationToken cancellationToken = default);
    Task<PageResult<Notification>> Handle(GetAllNotificationsQuery query, CancellationToken cancellationToken = default);
}
