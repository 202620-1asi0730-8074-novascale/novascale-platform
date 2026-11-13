using NovaScale.Platform.API.ConversationManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.Queries;
using NovaScale.Platform.API.ConversationManagement.Application.Services;
using NovaScale.Platform.API.ConversationManagement.Domain.Repositories;
using NovaScale.Platform.API.Shared.Application.Security;
using NovaScale.Platform.API.Shared.Application.Services;
using NovaScale.Platform.API.Shared.Application.Internal.QueryServices;
using NovaScale.Platform.API.Shared.Application.Patterns;

namespace NovaScale.Platform.API.ConversationManagement.Application.Internal.QueryServices;

public class AttachmentQueryService(IAttachmentRepository repository, IRequestContext request, IAccessPolicy access,
    IEntityLifecycle<Attachment> lifecycle) : CrudQueryService<Attachment>(repository, request, access, lifecycle), IAttachmentQueryService
{
    public Task<Attachment> Handle(GetAttachmentByIdQuery query, CancellationToken cancellationToken = default) =>
        FindAsync(query.Id, "read_conversations", cancellationToken);

    public Task<PageResult<Attachment>> Handle(GetAllAttachmentsQuery query, CancellationToken cancellationToken = default)
    {
        var search = query.Search?.Trim() ?? string.Empty;
        return ListAsync(query.Page, query.PageSize, entity => (search == "" || entity.Name.Contains(search) || entity.ContentType.Contains(search))
            && (!query.MessageId.HasValue || entity.MessageId == query.MessageId), "read_conversations", cancellationToken);
    }
}
