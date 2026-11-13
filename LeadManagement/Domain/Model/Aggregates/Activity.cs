using NovaScale.Platform.API.Shared.Domain.Model;
using NovaScale.Platform.API.LeadManagement.Domain.Model.Commands;

namespace NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates;

public class Activity : Entity
{
    protected Activity() { }

    public Activity(CreateActivityCommand command) => Update(new UpdateActivityCommand(command.Type, command.Content, command.UserId, command.ContactId));

    public string Type { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public long UserId { get; private set; }
    public long? ContactId { get; private set; }

    public void Update(UpdateActivityCommand command)
    {
        Type = Guard.Choice(command.Type, "CONTACT_CREATED", "CONTACT_UPDATED", "OPPORTUNITY_UPDATED", "FOLLOWUP", "STAGE_CHANGED");
        Content = Guard.Text(command.Content, 1000);
        UserId = command.UserId;
        ContactId = command.ContactId;
    }
}
