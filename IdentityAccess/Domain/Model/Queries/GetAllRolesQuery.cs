namespace NovaScale.Platform.API.IdentityAccess.Domain.Model.Queries;

public record GetAllRolesQuery(string? Search = null, int Page = 1, int PageSize = 20);
