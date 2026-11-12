using NovaScale.Platform.API.ConversationManagement.Domain.Model.Commands;
using NovaScale.Platform.API.ConversationManagement.Interfaces.REST.Resources;

namespace NovaScale.Platform.API.ConversationManagement.Interfaces.REST.Transform;

public static class UpdateConversationCommandFromResourceAssembler
{
    public static UpdateConversationCommand ToCommandFromResource(UpdateConversationResource resource) => new(resource.Channel, resource.Status, resource.ContactId, resource.UserId);
}
