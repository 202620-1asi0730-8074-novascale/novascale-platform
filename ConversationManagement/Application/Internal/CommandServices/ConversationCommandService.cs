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

public class ConversationCommandService(IConversationRepository repository, IUnitOfWork unitOfWork, IRequestContext request,
    IAccessPolicy access, IEntityLifecycle<Conversation> lifecycle)
    : CrudCommandService<Conversation>(repository, unitOfWork, request, access, lifecycle), IConversationCommandService
{
    public Task<Conversation> Handle(CreateConversationCommand command, CancellationToken cancellationToken = default) =>
        CreateAsync(() => new Conversation(command), "write_conversations", cancellationToken);

    public Task<Conversation> Handle(long id, UpdateConversationCommand command, CancellationToken cancellationToken = default) =>
        UpdateAsync(id, entity => entity.Update(command), "write_conversations", cancellationToken);

    public Task RemoveAsync(long id, CancellationToken cancellationToken = default) =>
        DeleteAsync(id, "write_conversations", cancellationToken);
}
