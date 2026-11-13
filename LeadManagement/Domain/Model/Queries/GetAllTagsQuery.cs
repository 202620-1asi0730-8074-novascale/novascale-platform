namespace NovaScale.Platform.API.LeadManagement.Domain.Model.Queries;

public record GetAllTagsQuery(string? Search = null, int Page = 1, int PageSize = 20);
