namespace NovaScale.Platform.API.LeadManagement.Domain.Model.Commands;

public record CreateContactCommand(string FullName, string Email, string Phone, string Company, string ContactType, string LeadStatus, string Source, long UserId, decimal? EstimatedAmount = null, IReadOnlyList<string>? Tags = null);
