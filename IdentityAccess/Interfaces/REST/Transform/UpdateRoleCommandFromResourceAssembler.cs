using NovaScale.Platform.API.IdentityAccess.Domain.Model.Commands;
using NovaScale.Platform.API.IdentityAccess.Interfaces.REST.Resources;

namespace NovaScale.Platform.API.IdentityAccess.Interfaces.REST.Transform;

public static class UpdateRoleCommandFromResourceAssembler
{
    public static UpdateRoleCommand ToCommandFromResource(UpdateRoleResource resource) => new(resource.Name, resource.Description ?? string.Empty);
}
