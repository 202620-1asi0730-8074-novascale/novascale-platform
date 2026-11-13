using NovaScale.Platform.API.IdentityAccess.Domain.Model.Commands;
using NovaScale.Platform.API.IdentityAccess.Interfaces.REST.Resources;

namespace NovaScale.Platform.API.IdentityAccess.Interfaces.REST.Transform;

public static class UpdatePermissionCommandFromResourceAssembler
{
    public static UpdatePermissionCommand ToCommandFromResource(UpdatePermissionResource resource) => new(resource.Code);
}
