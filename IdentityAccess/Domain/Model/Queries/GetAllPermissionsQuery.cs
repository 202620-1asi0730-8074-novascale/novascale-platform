namespace NovaScale.Platform.API.IdentityAccess.Domain.Model.Queries;

public record GetAllPermissionsQuery(string? Search = null, int Page = 1, int PageSize = 20);
