using NovaScale.Platform.API.IdentityAccess.Domain.Model.Commands;
using NovaScale.Platform.API.IdentityAccess.Interfaces.REST.Resources;

namespace NovaScale.Platform.API.IdentityAccess.Interfaces.REST.Transform;

public static class CreatePermissionCommandFromResourceAssembler
{
    public static CreatePermissionCommand ToCommandFromResource(CreatePermissionResource resource) => new(resource.Code);
}
