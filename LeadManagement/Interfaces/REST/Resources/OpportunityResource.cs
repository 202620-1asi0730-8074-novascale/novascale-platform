namespace NovaScale.Platform.API.LeadManagement.Interfaces.REST.Resources;

public record OpportunityResource(long Id, string Title, decimal EstimatedAmount, string Status, DateOnly? ExpectedCloseDate, long UserId, long ContactId, DateTime CreatedAt, DateTime UpdatedAt);
