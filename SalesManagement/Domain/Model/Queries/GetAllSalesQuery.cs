namespace NovaScale.Platform.API.SalesManagement.Domain.Model.Queries;

public record GetAllSalesQuery(string? Search = null, int Page = 1, int PageSize = 20);
