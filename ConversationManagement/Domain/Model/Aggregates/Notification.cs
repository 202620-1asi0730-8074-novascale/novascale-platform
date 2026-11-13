using NovaScale.Platform.API.Shared.Domain.Model;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.Commands;

namespace NovaScale.Platform.API.ConversationManagement.Domain.Model.Aggregates;

public class Notification : Entity
{
    protected Notification() { }

    public Notification(CreateNotificationCommand command) => Update(new UpdateNotificationCommand(command.Type, command.Content, command.IsRead, command.UserId, command.ConversationId));

    public string Type { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public bool IsRead { get; private set; }
    public long UserId { get; private set; }
    public long ConversationId { get; private set; }

    public void Update(UpdateNotificationCommand command)
    {
        Type = Guard.Choice(command.Type, "NEW_MESSAGE", "PENDING_REPLY");
        Content = Guard.Text(command.Content, 255);
        IsRead = command.IsRead;
        UserId = command.UserId;
        ConversationId = command.ConversationId;
    }
}
