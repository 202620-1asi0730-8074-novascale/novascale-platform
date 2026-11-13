using NovaScale.Platform.API.IdentityAccess.Domain.Model.Commands;
using NovaScale.Platform.API.IdentityAccess.Interfaces.REST.Resources;

namespace NovaScale.Platform.API.IdentityAccess.Interfaces.REST.Transform;

public static class CreateRoleCommandFromResourceAssembler
{
    public static CreateRoleCommand ToCommandFromResource(CreateRoleResource resource) => new(resource.Name, resource.Description ?? string.Empty);
}
