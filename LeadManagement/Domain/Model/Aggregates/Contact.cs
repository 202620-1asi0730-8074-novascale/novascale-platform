using NovaScale.Platform.API.Shared.Domain.Model;
using NovaScale.Platform.API.LeadManagement.Domain.Model.Commands;

namespace NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates;

public class Contact : Entity
{
    protected Contact() { }

    public Contact(CreateContactCommand command) => Update(new UpdateContactCommand(command.FullName, command.Email, command.Phone, command.Company, command.ContactType, command.LeadStatus, command.Source, command.UserId));

    public string FullName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public string Company { get; private set; } = string.Empty;
    public string ContactType { get; private set; } = string.Empty;
    public string LeadStatus { get; private set; } = string.Empty;
    public string Source { get; private set; } = string.Empty;
    public long UserId { get; private set; }

    public void ChangeStage(string stage)
    {
        LeadStatus = Guard.Choice(stage, "NEW", "CONTACTED", "PROPOSAL", "WON");
        ContactType = LeadStatus == "WON" ? "CUSTOMER" : "LEAD";
    }

    public void Update(UpdateContactCommand command)
    {
        FullName = Guard.Text(command.FullName, 120);
        Email = Guard.Email(command.Email, false);
        Phone = Guard.Phone(command.Phone);
        Company = Guard.Text(command.Company, 120, false);
        ContactType = Guard.Choice(command.ContactType, "LEAD", "CUSTOMER");
        LeadStatus = Guard.Choice(command.LeadStatus, "NEW", "CONTACTED", "PROPOSAL", "WON");
        Source = Guard.Text(command.Source, 80, false);
        UserId = command.UserId;
    }
}
