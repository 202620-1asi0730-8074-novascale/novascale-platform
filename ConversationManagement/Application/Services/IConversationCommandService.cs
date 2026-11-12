using NovaScale.Platform.API.ConversationManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.Commands;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.Queries;

namespace NovaScale.Platform.API.ConversationManagement.Application.Services;

public interface IConversationCommandService
{
    Task<Conversation> Handle(CreateConversationCommand command, CancellationToken cancellationToken = default);
    Task<Conversation> Handle(long id, UpdateConversationCommand command, CancellationToken cancellationToken = default);
    Task RemoveAsync(long id, CancellationToken cancellationToken = default);
}
