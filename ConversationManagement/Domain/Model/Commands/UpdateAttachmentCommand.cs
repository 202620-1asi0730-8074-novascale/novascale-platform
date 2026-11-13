namespace NovaScale.Platform.API.ConversationManagement.Domain.Model.Commands;

public record UpdateAttachmentCommand(string Name, string ContentType, string Data, long MessageId);
