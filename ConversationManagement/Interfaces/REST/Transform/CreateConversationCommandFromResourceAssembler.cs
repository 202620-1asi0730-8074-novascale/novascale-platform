using NovaScale.Platform.API.ConversationManagement.Domain.Model.Commands;
using NovaScale.Platform.API.ConversationManagement.Interfaces.REST.Resources;

namespace NovaScale.Platform.API.ConversationManagement.Interfaces.REST.Transform;

public static class CreateConversationCommandFromResourceAssembler
{
    public static CreateConversationCommand ToCommandFromResource(CreateConversationResource resource) => new(resource.Channel, resource.Status, resource.ContactId, resource.UserId);
}
