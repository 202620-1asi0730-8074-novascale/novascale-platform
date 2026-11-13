using NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates;
using NovaScale.Platform.API.IdentityAccess.Interfaces.REST.Resources;

namespace NovaScale.Platform.API.IdentityAccess.Interfaces.REST.Transform;

public static class UserResourceFromEntityAssembler
{
    public static UserResource ToResourceFromEntity(User entity) => new(entity.Id, entity.FullName, entity.Email, entity.Phone, entity.Quota, entity.Status, entity.RoleId, entity.CanExportData, entity.CanDeleteLeads, entity.CreatedAt, entity.UpdatedAt);
}
