namespace NovaScale.Platform.API.SalesManagement.Interfaces.REST.Resources;

public record SaleResource(long Id, decimal Amount, DateOnly SaleDate, string Status, string Notes, long UserId, long OpportunityId, DateTime CreatedAt, DateTime UpdatedAt);
