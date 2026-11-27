using NovaScale.Platform.API.Shared.Domain.Model;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.Commands;

namespace NovaScale.Platform.API.ConversationManagement.Domain.Model.Aggregates;

public class Message : Entity
{
    protected Message() { }

    public Message(CreateMessageCommand command) => Update(new UpdateMessageCommand(command.SenderType, command.Content, command.ConversationId));

    public string SenderType { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public long ConversationId { get; private set; }

    public long UserId { get; private set; }
    public DateTime SentAt { get; private set; } = DateTime.UtcNow;
    public string? ExternalMessageId { get; private set; }
    public string DeliveryStatus { get; private set; } = "LOCAL";
    public string? EmailMessageId { get; private set; }
    public Guid? ClientRequestId { get; private set; }
    public void PrepareEmail(Guid requestId, string emailMessageId)
    {
        ClientRequestId = requestId;
        EmailMessageId = emailMessageId is { Length: <= 255 } ? emailMessageId : null;
        ExternalMessageId = "pending:" + requestId.ToString("N");
        DeliveryStatus = "PENDING";
    }
    public void ApplyEmail(string externalId, string? emailMessageId, DateTime sentAt)
    {
        ExternalMessageId = externalId;
        EmailMessageId = emailMessageId is { Length: <= 255 } ? emailMessageId : null;
        SentAt = sentAt;
        DeliveryStatus = SenderType == "OUT" ? "SENT" : "RECEIVED";
    }
    public void MarkDeliveryUncertain() => DeliveryStatus = "UNKNOWN";
    public void MarkDeliveryFailed() => DeliveryStatus = "FAILED";
    public ICollection<Attachment> Attachments { get; private set; } = new List<Attachment>();
    public void AssignUser(long id) => UserId = id;
    public void SetAttachments(IEnumerable<Attachment> attachments)
    {
        Attachments.Clear();
        foreach (var attachment in attachments) Attachments.Add(attachment);
    }

    public void Update(UpdateMessageCommand command)
    {
        SenderType = Guard.Choice(command.SenderType, "IN", "OUT");
        Content = Guard.Text(command.Content, 16000, false);
        ConversationId = command.ConversationId;
    }
}
