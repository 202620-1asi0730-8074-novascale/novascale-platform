namespace NovaScale.Platform.API.LeadManagement.Interfaces.REST.Resources;

public record ContactResource(long Id, string FullName, string Email, string Phone, string Company, string ContactType, string LeadStatus, string Source, long UserId, DateTime CreatedAt, DateTime UpdatedAt, decimal EstimatedAmount = 0, long? OpportunityId = null, IReadOnlyList<string>? Tags = null, string Owner = "", DateTime? WonAt = null);
