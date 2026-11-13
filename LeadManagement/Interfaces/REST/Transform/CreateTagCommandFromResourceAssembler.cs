using NovaScale.Platform.API.LeadManagement.Domain.Model.Commands;
using NovaScale.Platform.API.LeadManagement.Interfaces.REST.Resources;

namespace NovaScale.Platform.API.LeadManagement.Interfaces.REST.Transform;

public static class CreateTagCommandFromResourceAssembler
{
    public static CreateTagCommand ToCommandFromResource(CreateTagResource resource) => new(resource.Name, resource.Color);
}
