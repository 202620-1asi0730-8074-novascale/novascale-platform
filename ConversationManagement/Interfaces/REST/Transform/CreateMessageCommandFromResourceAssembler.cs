using NovaScale.Platform.API.ConversationManagement.Domain.Model.Commands;
using NovaScale.Platform.API.ConversationManagement.Interfaces.REST.Resources;

namespace NovaScale.Platform.API.ConversationManagement.Interfaces.REST.Transform;

public static class CreateMessageCommandFromResourceAssembler
{
    public static CreateMessageCommand ToCommandFromResource(CreateMessageResource resource) => new(resource.SenderType, resource.Content ?? string.Empty, resource.ConversationId);
}
