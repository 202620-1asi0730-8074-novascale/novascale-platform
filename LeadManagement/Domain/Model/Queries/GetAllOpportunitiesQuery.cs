namespace NovaScale.Platform.API.LeadManagement.Domain.Model.Queries;

public record GetAllOpportunitiesQuery(string? Search = null, int Page = 1, int PageSize = 20, string? Status = null, long? UserId = null, long? ContactId = null);
