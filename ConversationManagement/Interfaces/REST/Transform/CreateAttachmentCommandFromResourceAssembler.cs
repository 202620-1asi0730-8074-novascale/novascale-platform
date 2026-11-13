using NovaScale.Platform.API.ConversationManagement.Domain.Model.Commands;
using NovaScale.Platform.API.ConversationManagement.Interfaces.REST.Resources;

namespace NovaScale.Platform.API.ConversationManagement.Interfaces.REST.Transform;

public static class CreateAttachmentCommandFromResourceAssembler
{
    public static CreateAttachmentCommand ToCommandFromResource(CreateAttachmentResource resource) => new(resource.Name, resource.ContentType, resource.Data, resource.MessageId);
}
