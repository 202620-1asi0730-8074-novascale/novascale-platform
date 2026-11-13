using NovaScale.Platform.API.ConversationManagement.Domain.Model.ValueObjects;

using NovaScale.Platform.API.ConversationManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.ConversationManagement.Interfaces.REST.Resources;

namespace NovaScale.Platform.API.ConversationManagement.Interfaces.REST.Transform;

public static class MessageResourceFromEntityAssembler
{
    public static MessageResource ToResourceFromEntity(Message entity) => new(entity.Id, entity.SenderType, entity.Content, entity.ConversationId, entity.CreatedAt, entity.UpdatedAt, entity.SentAt, entity.ExternalMessageId, entity.Attachments.OrderBy(attachment => attachment.Id).Select(attachment => new MessageAttachment(attachment.Name, attachment.ContentType, attachment.Data)).ToArray());
}
