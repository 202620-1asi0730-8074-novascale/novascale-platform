using NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.LeadManagement.Interfaces.REST.Resources;

namespace NovaScale.Platform.API.LeadManagement.Interfaces.REST.Transform;

public static class ActivityResourceFromEntityAssembler
{
    public static ActivityResource ToResourceFromEntity(Activity entity) => new(entity.Id, entity.Type, entity.Content, entity.UserId, entity.ContactId, entity.CreatedAt, entity.UpdatedAt);
}
