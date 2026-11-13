using NovaScale.Platform.API.ConversationManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.Commands;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.ValueObjects;

namespace NovaScale.Platform.API.ConversationManagement.Application.Services;

public interface IMessageDeliveryService
{
    Task<Message> CreateAsync(CreateMessageCommand command, IReadOnlyList<MessageAttachment>? attachments, CancellationToken cancellationToken = default);
    Task<Message> UpdateAsync(long id, UpdateMessageCommand command, IReadOnlyList<MessageAttachment>? attachments, CancellationToken cancellationToken = default);
}
