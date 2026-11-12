namespace NovaScale.Platform.API.ConversationManagement.Domain.Model.Commands;

public record UpdateNotificationCommand(string Type, string Content, bool IsRead, long UserId, long ConversationId);
