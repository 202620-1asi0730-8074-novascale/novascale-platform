namespace NovaScale.Platform.API.ConversationManagement.Interfaces.REST.Resources;

public record NotificationResource(long Id, string Type, string Content, bool IsRead, long UserId, long ConversationId, DateTime CreatedAt, DateTime UpdatedAt);
