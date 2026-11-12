using NovaScale.Platform.API.OrganizationManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.OrganizationManagement.Interfaces.REST.Resources;

namespace NovaScale.Platform.API.OrganizationManagement.Interfaces.REST.Transform;

public static class OrganizationResourceFromEntityAssembler
{
    public static OrganizationResource ToResourceFromEntity(Organization entity) => new(entity.Id, entity.Name, entity.CreatedAt);
}
