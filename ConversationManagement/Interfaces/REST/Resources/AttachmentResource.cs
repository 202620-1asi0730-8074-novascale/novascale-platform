namespace NovaScale.Platform.API.ConversationManagement.Interfaces.REST.Resources;

public record AttachmentResource(long Id, long MessageId, string Name, string ContentType, string Data, int SizeBytes, DateTime CreatedAt, DateTime UpdatedAt);
