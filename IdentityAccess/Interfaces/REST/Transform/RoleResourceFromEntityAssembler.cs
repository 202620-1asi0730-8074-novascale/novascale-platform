using NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates;
using NovaScale.Platform.API.IdentityAccess.Interfaces.REST.Resources;

namespace NovaScale.Platform.API.IdentityAccess.Interfaces.REST.Transform;

public static class RoleResourceFromEntityAssembler
{
    public static RoleResource ToResourceFromEntity(Role entity) => new(entity.Id, entity.Name, entity.Description, entity.CreatedAt, entity.UpdatedAt, entity.SystemCode);
}
