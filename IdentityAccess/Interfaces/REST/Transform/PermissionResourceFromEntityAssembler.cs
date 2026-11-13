using NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates;
using NovaScale.Platform.API.IdentityAccess.Interfaces.REST.Resources;

namespace NovaScale.Platform.API.IdentityAccess.Interfaces.REST.Transform;

public static class PermissionResourceFromEntityAssembler
{
    public static PermissionResource ToResourceFromEntity(Permission entity) => new(entity.Id, entity.Code, entity.CreatedAt, entity.UpdatedAt);
}
