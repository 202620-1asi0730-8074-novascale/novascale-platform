using Microsoft.EntityFrameworkCore;
using NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.Commands;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.Queries;
using NovaScale.Platform.API.ConversationManagement.Application.Services;
using NovaScale.Platform.API.ConversationManagement.Domain.Repositories;
using NovaScale.Platform.API.Shared.Application.Security;
using NovaScale.Platform.API.Shared.Application.Services;
using NovaScale.Platform.API.Shared.Domain.Repositories;
using NovaScale.Platform.API.Shared.Application.Internal.QueryServices;
using NovaScale.Platform.API.Shared.Application.Patterns;

namespace NovaScale.Platform.API.ConversationManagement.Application.Internal.QueryServices;

public class NotificationQueryService(INotificationRepository repository, IRequestContext request, IAccessPolicy access,
    IEntityLifecycle<Notification> lifecycle) : CrudQueryService<Notification>(repository, request, access, lifecycle), INotificationQueryService
{
    public Task<Notification> Handle(GetNotificationByIdQuery query, CancellationToken cancellationToken = default) =>
        FindAsync(query.Id, "read_conversations", cancellationToken);

    public Task<PageResult<Notification>> Handle(GetAllNotificationsQuery query, CancellationToken cancellationToken = default)
    {
        var search = query.Search?.Trim() ?? string.Empty;
        return ListAsync(query.Page, query.PageSize, entity => (search == "" || entity.Type.Contains(search) || entity.Content.Contains(search)) && (!query.IsRead.HasValue || entity.IsRead == query.IsRead) && (!query.ConversationId.HasValue || entity.ConversationId == query.ConversationId), "read_conversations", cancellationToken);
    }
}
