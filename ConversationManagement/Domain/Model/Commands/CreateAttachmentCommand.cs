namespace NovaScale.Platform.API.ConversationManagement.Domain.Model.Commands;

public record CreateAttachmentCommand(string Name, string ContentType, string Data, long MessageId);
