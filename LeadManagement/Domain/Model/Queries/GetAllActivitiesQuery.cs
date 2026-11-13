namespace NovaScale.Platform.API.LeadManagement.Domain.Model.Queries;

public record GetAllActivitiesQuery(string? Search = null, int Page = 1, int PageSize = 20);
