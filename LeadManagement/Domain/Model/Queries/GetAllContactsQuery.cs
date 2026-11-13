namespace NovaScale.Platform.API.LeadManagement.Domain.Model.Queries;

public record GetAllContactsQuery(string? Search = null, int Page = 1, int PageSize = 20, string? ContactType = null, string? Status = null, long? UserId = null, long? TagId = null);
