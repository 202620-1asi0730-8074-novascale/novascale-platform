using NovaScale.Platform.API.Shared.Domain.Model;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.Commands;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.ValueObjects;

namespace NovaScale.Platform.API.ConversationManagement.Domain.Model.Aggregates;

public class Attachment : Entity
{
    protected Attachment() { }

    public Attachment(CreateAttachmentCommand command) => Update(new UpdateAttachmentCommand(command.Name, command.ContentType, command.Data, command.MessageId));

    public string Name { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public string Data { get; private set; } = string.Empty;
    public int SizeBytes { get; private set; }
    public long MessageId { get; private set; }

    public void Update(UpdateAttachmentCommand command)
    {
        SizeBytes = new MessageAttachment(command.Name, command.ContentType, command.Data).Validate();
        Name = command.Name;
        ContentType = command.ContentType;
        Data = command.Data;
        MessageId = command.MessageId;
    }
}
