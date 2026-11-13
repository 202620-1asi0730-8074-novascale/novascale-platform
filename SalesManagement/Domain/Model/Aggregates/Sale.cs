using NovaScale.Platform.API.Shared.Domain.Model;
using NovaScale.Platform.API.SalesManagement.Domain.Model.Commands;

namespace NovaScale.Platform.API.SalesManagement.Domain.Model.Aggregates;

public class Sale : Entity
{
    protected Sale() { }

    public Sale(CreateSaleCommand command) => Update(new UpdateSaleCommand(command.Amount, command.SaleDate, command.Status, command.Notes, command.UserId, command.OpportunityId));

    public decimal Amount { get; private set; }
    public DateOnly SaleDate { get; private set; }
    public string Status { get; private set; } = string.Empty;
    public string Notes { get; private set; } = string.Empty;
    public long UserId { get; private set; }
    public long OpportunityId { get; private set; }

    public void Update(UpdateSaleCommand command)
    {
        Amount = Guard.Amount(command.Amount);
        SaleDate = Guard.Date(command.SaleDate);
        Status = Guard.Choice(command.Status, "CONFIRMED", "CANCELLED");
        Notes = Guard.Text(command.Notes, 4000, false);
        UserId = command.UserId;
        OpportunityId = command.OpportunityId;
    }
}
