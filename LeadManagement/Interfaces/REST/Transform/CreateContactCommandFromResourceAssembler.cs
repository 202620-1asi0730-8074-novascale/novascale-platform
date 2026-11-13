using NovaScale.Platform.API.LeadManagement.Domain.Model.Commands;
using NovaScale.Platform.API.LeadManagement.Interfaces.REST.Resources;

namespace NovaScale.Platform.API.LeadManagement.Interfaces.REST.Transform;

public static class CreateContactCommandFromResourceAssembler
{
    public static CreateContactCommand ToCommandFromResource(CreateContactResource resource) => new(resource.FullName, resource.Email ?? string.Empty, resource.Phone ?? string.Empty, resource.Company ?? string.Empty, resource.ContactType, resource.LeadStatus, resource.Source ?? string.Empty, resource.UserId, resource.EstimatedAmount, resource.Tags);
}
