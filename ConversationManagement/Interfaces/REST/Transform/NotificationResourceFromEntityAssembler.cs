using NovaScale.Platform.API.ConversationManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.ConversationManagement.Interfaces.REST.Resources;

namespace NovaScale.Platform.API.ConversationManagement.Interfaces.REST.Transform;

public static class NotificationResourceFromEntityAssembler
{
    public static NotificationResource ToResourceFromEntity(Notification entity) => new(entity.Id, entity.Type, entity.Content, entity.IsRead, entity.UserId, entity.ConversationId, entity.CreatedAt, entity.UpdatedAt);
}
