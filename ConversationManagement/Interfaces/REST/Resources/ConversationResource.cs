namespace NovaScale.Platform.API.ConversationManagement.Interfaces.REST.Resources;

public record ConversationResource(long Id, string Channel, string Status, long ContactId, long UserId, DateTime CreatedAt, DateTime UpdatedAt, string Priority, DateTime? PendingSince, DateTime? LastMessageAt);
