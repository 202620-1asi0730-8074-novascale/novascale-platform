using NovaScale.Platform.API.ConversationManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.ConversationManagement.Interfaces.REST.Resources;

namespace NovaScale.Platform.API.ConversationManagement.Interfaces.REST.Transform;

public static class ConversationResourceFromEntityAssembler
{
    public static ConversationResource ToResourceFromEntity(Conversation entity) => new(entity.Id, entity.Channel, entity.Status, entity.ContactId, entity.UserId, entity.CreatedAt, entity.UpdatedAt, entity.Priority, entity.PendingSince, entity.LastMessageAt);
}
