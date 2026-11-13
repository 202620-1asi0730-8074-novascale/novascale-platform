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

public class MessageCommandService(IMessageRepository repository, IUnitOfWork unitOfWork, IRequestContext request,
    IAccessPolicy access, IEntityLifecycle<Message> lifecycle)
    : CrudCommandService<Message>(repository, unitOfWork, request, access, lifecycle), IMessageCommandService
{
    public Task<Message> Handle(CreateMessageCommand command, CancellationToken cancellationToken = default) =>
        CreateAsync(() => new Message(command), "write_conversations", cancellationToken);

    public Task<Message> Handle(long id, UpdateMessageCommand command, CancellationToken cancellationToken = default) =>
        UpdateAsync(id, entity => entity.Update(command), "write_conversations", cancellationToken);

    public Task RemoveAsync(long id, CancellationToken cancellationToken = default) =>
        DeleteAsync(id, "write_conversations", cancellationToken);
}
