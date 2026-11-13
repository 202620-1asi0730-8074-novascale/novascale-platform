namespace NovaScale.Platform.API.IdentityAccess.Domain.Model.Commands;

public record UpdateUserCommand(string FullName, string Email, string Phone, decimal Quota, string Status, long RoleId, bool CanExportData, bool CanDeleteLeads);
