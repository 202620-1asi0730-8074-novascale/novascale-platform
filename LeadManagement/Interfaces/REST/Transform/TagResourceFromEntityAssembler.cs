using NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.LeadManagement.Interfaces.REST.Resources;

namespace NovaScale.Platform.API.LeadManagement.Interfaces.REST.Transform;

public static class TagResourceFromEntityAssembler
{
    public static TagResource ToResourceFromEntity(Tag entity) => new(entity.Id, entity.Name, entity.Color, entity.CreatedAt, entity.UpdatedAt);
}
