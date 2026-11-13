using NovaScale.Platform.API.ConversationManagement.Domain.Model.Commands;
using NovaScale.Platform.API.ConversationManagement.Interfaces.REST.Resources;

namespace NovaScale.Platform.API.ConversationManagement.Interfaces.REST.Transform;

public static class CreateNotificationCommandFromResourceAssembler
{
    public static CreateNotificationCommand ToCommandFromResource(CreateNotificationResource resource) => new(resource.Type, resource.Content, resource.IsRead, resource.UserId, resource.ConversationId);
}
