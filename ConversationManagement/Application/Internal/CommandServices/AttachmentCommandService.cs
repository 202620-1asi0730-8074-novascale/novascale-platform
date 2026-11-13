using NovaScale.Platform.API.ConversationManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.Commands;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.Queries;
using NovaScale.Platform.API.ConversationManagement.Application.Services;
using NovaScale.Platform.API.ConversationManagement.Domain.Repositories;
using NovaScale.Platform.API.Shared.Application.Security;
using NovaScale.Platform.API.Shared.Application.Services;
using NovaScale.Platform.API.Shared.Domain.Repositories;
using NovaScale.Platform.API.Shared.Application.Internal.CommandServices;

namespace NovaScale.Platform.API.ConversationManagement.Application.Internal.CommandServices;

public class AttachmentCommandService(IAttachmentRepository repository, IUnitOfWork unitOfWork, IRequestContext request,
    IAccessPolicy access, IEntityLifecycle<Attachment> lifecycle)
    : CrudCommandService<Attachment>(repository, unitOfWork, request, access, lifecycle), IAttachmentCommandService
{
    public Task<Attachment> Handle(CreateAttachmentCommand command, CancellationToken cancellationToken = default) =>
        CreateAsync(() => new Attachment(command), "write_conversations", cancellationToken);

    public Task<Attachment> Handle(long id, UpdateAttachmentCommand command, CancellationToken cancellationToken = default) =>
        UpdateAsync(id, entity => entity.Update(command), "write_conversations", cancellationToken);

    public Task RemoveAsync(long id, CancellationToken cancellationToken = default) =>
        DeleteAsync(id, "write_conversations", cancellationToken);
}
