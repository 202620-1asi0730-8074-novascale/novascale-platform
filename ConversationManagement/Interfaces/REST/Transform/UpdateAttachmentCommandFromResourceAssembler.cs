using NovaScale.Platform.API.ConversationManagement.Domain.Model.Commands;
using NovaScale.Platform.API.ConversationManagement.Interfaces.REST.Resources;

namespace NovaScale.Platform.API.ConversationManagement.Interfaces.REST.Transform;

public static class UpdateAttachmentCommandFromResourceAssembler
{
    public static UpdateAttachmentCommand ToCommandFromResource(UpdateAttachmentResource resource) => new(resource.Name, resource.ContentType, resource.Data, resource.MessageId);
}
