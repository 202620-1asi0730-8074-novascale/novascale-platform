using NovaScale.Platform.API.ConversationManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.Commands;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.Queries;

namespace NovaScale.Platform.API.ConversationManagement.Application.Services;

public interface IMessageCommandService
{
    Task<Message> Handle(CreateMessageCommand command, CancellationToken cancellationToken = default);
    Task<Message> Handle(long id, UpdateMessageCommand command, CancellationToken cancellationToken = default);
    Task RemoveAsync(long id, CancellationToken cancellationToken = default);
}
