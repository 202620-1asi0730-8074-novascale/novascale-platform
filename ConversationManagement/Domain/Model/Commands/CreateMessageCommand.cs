namespace NovaScale.Platform.API.ConversationManagement.Domain.Model.Commands;

public record CreateMessageCommand(string SenderType, string Content, long ConversationId);
