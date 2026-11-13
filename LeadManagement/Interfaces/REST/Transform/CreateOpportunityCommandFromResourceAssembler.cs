using NovaScale.Platform.API.LeadManagement.Domain.Model.Commands;
using NovaScale.Platform.API.LeadManagement.Interfaces.REST.Resources;

namespace NovaScale.Platform.API.LeadManagement.Interfaces.REST.Transform;

public static class CreateOpportunityCommandFromResourceAssembler
{
    public static CreateOpportunityCommand ToCommandFromResource(CreateOpportunityResource resource) => new(resource.Title, resource.EstimatedAmount, resource.Status, resource.ExpectedCloseDate, resource.UserId, resource.ContactId);
}
