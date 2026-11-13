namespace NovaScale.Platform.API.IdentityAccess.Interfaces.REST.Resources;

public record UserResource(long Id, string FullName, string Email, string Phone, decimal Quota, string Status, long RoleId, bool CanExportData, bool CanDeleteLeads, DateTime CreatedAt, DateTime UpdatedAt);
