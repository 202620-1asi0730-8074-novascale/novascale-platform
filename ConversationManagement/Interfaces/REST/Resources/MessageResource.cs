using NovaScale.Platform.API.ConversationManagement.Domain.Model.ValueObjects;

namespace NovaScale.Platform.API.ConversationManagement.Interfaces.REST.Resources;

public record MessageResource(long Id, string SenderType, string Content, long ConversationId, DateTime CreatedAt, DateTime UpdatedAt, DateTime SentAt, string? ExternalMessageId, IReadOnlyList<MessageAttachment> Attachments);
