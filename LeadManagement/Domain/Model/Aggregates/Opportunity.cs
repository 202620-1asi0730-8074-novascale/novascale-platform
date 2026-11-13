using NovaScale.Platform.API.Shared.Domain.Model;
using NovaScale.Platform.API.LeadManagement.Domain.Model.Commands;

namespace NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates;

public class Opportunity : Entity
{
    protected Opportunity() { }

    public Opportunity(CreateOpportunityCommand command) => Update(new UpdateOpportunityCommand(command.Title, command.EstimatedAmount, command.Status, command.ExpectedCloseDate, command.UserId, command.ContactId));

    public string Title { get; private set; } = string.Empty;
    public decimal EstimatedAmount { get; private set; }
    public string Status { get; private set; } = string.Empty;
    public DateOnly? ExpectedCloseDate { get; private set; }
    public long UserId { get; private set; }
    public long ContactId { get; private set; }

    public void Update(UpdateOpportunityCommand command)
    {
        Title = Guard.Text(command.Title, 150);
        EstimatedAmount = Guard.Amount(command.EstimatedAmount);
        Status = Guard.Choice(command.Status, "NEW", "CONTACTED", "PROPOSAL", "WON", "LOST");
        ExpectedCloseDate = command.ExpectedCloseDate.HasValue ? Guard.Date(command.ExpectedCloseDate.Value) : null;
        UserId = command.UserId;
        ContactId = command.ContactId;
    }
}
