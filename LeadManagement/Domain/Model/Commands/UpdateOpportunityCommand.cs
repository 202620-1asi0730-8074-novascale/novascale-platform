namespace NovaScale.Platform.API.LeadManagement.Domain.Model.Commands;

public record UpdateOpportunityCommand(string Title, decimal EstimatedAmount, string Status, DateOnly? ExpectedCloseDate, long UserId, long ContactId);
