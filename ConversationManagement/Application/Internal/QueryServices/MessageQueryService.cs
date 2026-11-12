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

public class MessageQueryService(IMessageRepository repository, IRequestContext request, IAccessPolicy access,
    IEntityLifecycle<Message> lifecycle) : CrudQueryService<Message>(repository, request, access, lifecycle), IMessageQueryService
{
    public Task<Message> Handle(GetMessageByIdQuery query, CancellationToken cancellationToken = default) =>
        FindAsync(query.Id, "read_conversations", cancellationToken);

    public Task<PageResult<Message>> Handle(GetAllMessagesQuery query, CancellationToken cancellationToken = default)
    {
        var search = query.Search?.Trim() ?? string.Empty;
        return ListAsync(query.Page, query.PageSize, entity => (search == "" || entity.SenderType.Contains(search) || entity.Content.Contains(search)) && (!query.ConversationId.HasValue || entity.ConversationId == query.ConversationId), "read_conversations", cancellationToken);
    }
}
