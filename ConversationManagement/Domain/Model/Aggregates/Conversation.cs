using NovaScale.Platform.API.Shared.Domain.Model;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.Commands;

namespace NovaScale.Platform.API.ConversationManagement.Domain.Model.Aggregates;

public class Conversation : Entity
{
    protected Conversation() { }

    public Conversation(CreateConversationCommand command) => Update(new UpdateConversationCommand(command.Channel, command.Status, command.ContactId, command.UserId));

    public string Channel { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty;
    public long ContactId { get; private set; }
    public long UserId { get; private set; }

    public string Priority { get; private set; } = "NORMAL";
    public DateTime? PendingSince { get; private set; }
    public DateTime? LastMessageAt { get; private set; }
    public void UpdateTimeline(DateTime? pendingSince, DateTime? lastMessageAt, int thresholdMinutes)
    {
        PendingSince = pendingSince;
        LastMessageAt = lastMessageAt;
        Priority = pendingSince.HasValue && DateTime.UtcNow - pendingSince.Value >= TimeSpan.FromMinutes(thresholdMinutes) ? "HIGH" : "NORMAL";
    }

    public void Update(UpdateConversationCommand command)
    {
        Channel = Guard.Choice(command.Channel, "SIMULATED");
        Status = Guard.Choice(command.Status, "OPEN", "CLOSED");
        ContactId = command.ContactId;
        UserId = command.UserId;
    }
}
