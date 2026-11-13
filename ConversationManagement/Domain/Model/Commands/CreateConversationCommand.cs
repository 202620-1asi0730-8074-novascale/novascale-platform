namespace NovaScale.Platform.API.ConversationManagement.Domain.Model.Commands;

public record CreateConversationCommand(string Channel, string Status, long ContactId, long UserId);
