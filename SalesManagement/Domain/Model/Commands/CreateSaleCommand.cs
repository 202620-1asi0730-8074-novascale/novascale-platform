namespace NovaScale.Platform.API.SalesManagement.Domain.Model.Commands;

public record CreateSaleCommand(decimal Amount, DateOnly SaleDate, string Status, string Notes, long UserId, long OpportunityId);
