namespace NovaScale.Platform.API.ConversationManagement.Domain.Model.Commands;

public record UpdateMessageCommand(string SenderType, string Content, long ConversationId);
