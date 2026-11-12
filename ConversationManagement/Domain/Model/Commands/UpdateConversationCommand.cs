namespace NovaScale.Platform.API.ConversationManagement.Domain.Model.Commands;

public record UpdateConversationCommand(string Channel, string Status, long ContactId, long UserId);
