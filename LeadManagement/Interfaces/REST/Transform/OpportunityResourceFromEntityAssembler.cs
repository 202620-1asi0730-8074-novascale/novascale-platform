using NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.LeadManagement.Interfaces.REST.Resources;

namespace NovaScale.Platform.API.LeadManagement.Interfaces.REST.Transform;

public static class OpportunityResourceFromEntityAssembler
{
    public static OpportunityResource ToResourceFromEntity(Opportunity entity) => new(entity.Id, entity.Title, entity.EstimatedAmount, entity.Status, entity.ExpectedCloseDate, entity.UserId, entity.ContactId, entity.CreatedAt, entity.UpdatedAt);
}
