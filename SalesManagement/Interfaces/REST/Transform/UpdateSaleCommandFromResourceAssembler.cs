using NovaScale.Platform.API.SalesManagement.Domain.Model.Commands;
using NovaScale.Platform.API.SalesManagement.Interfaces.REST.Resources;

namespace NovaScale.Platform.API.SalesManagement.Interfaces.REST.Transform;

public static class UpdateSaleCommandFromResourceAssembler
{
    public static UpdateSaleCommand ToCommandFromResource(UpdateSaleResource resource) => new(resource.Amount, resource.SaleDate, resource.Status, resource.Notes ?? string.Empty, resource.UserId, resource.OpportunityId);
}
