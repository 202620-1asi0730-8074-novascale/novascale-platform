using NovaScale.Platform.API.IdentityAccess.Domain.Model.Commands;
using NovaScale.Platform.API.IdentityAccess.Interfaces.REST.Resources;

namespace NovaScale.Platform.API.IdentityAccess.Interfaces.REST.Transform;

public static class UpdateUserCommandFromResourceAssembler
{
    public static UpdateUserCommand ToCommandFromResource(UpdateUserResource resource) => new(resource.FullName, resource.Email, resource.Phone ?? string.Empty, resource.Quota, resource.Status, resource.RoleId, resource.CanExportData, resource.CanDeleteLeads);
}
