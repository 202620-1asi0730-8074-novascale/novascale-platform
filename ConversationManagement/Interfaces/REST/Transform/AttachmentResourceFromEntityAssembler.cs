using NovaScale.Platform.API.ConversationManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.ConversationManagement.Interfaces.REST.Resources;

namespace NovaScale.Platform.API.ConversationManagement.Interfaces.REST.Transform;

public static class AttachmentResourceFromEntityAssembler
{
    public static AttachmentResource ToResourceFromEntity(Attachment entity) => new(entity.Id, entity.MessageId, entity.Name, entity.ContentType, entity.Data, entity.SizeBytes, entity.CreatedAt, entity.UpdatedAt);
}
