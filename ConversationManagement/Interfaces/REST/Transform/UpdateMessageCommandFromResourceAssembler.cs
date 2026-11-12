using NovaScale.Platform.API.ConversationManagement.Domain.Model.Commands;
using NovaScale.Platform.API.ConversationManagement.Interfaces.REST.Resources;

namespace NovaScale.Platform.API.ConversationManagement.Interfaces.REST.Transform;

public static class UpdateMessageCommandFromResourceAssembler
{
    public static UpdateMessageCommand ToCommandFromResource(UpdateMessageResource resource) => new(resource.SenderType, resource.Content ?? string.Empty, resource.ConversationId);
}
