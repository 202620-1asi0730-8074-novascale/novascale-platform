using NovaScale.Platform.API.SalesManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.SalesManagement.Interfaces.REST.Resources;

namespace NovaScale.Platform.API.SalesManagement.Interfaces.REST.Transform;

public static class SaleResourceFromEntityAssembler
{
    public static SaleResource ToResourceFromEntity(Sale entity) => new(entity.Id, entity.Amount, entity.SaleDate, entity.Status, entity.Notes, entity.UserId, entity.OpportunityId, entity.CreatedAt, entity.UpdatedAt);
}
