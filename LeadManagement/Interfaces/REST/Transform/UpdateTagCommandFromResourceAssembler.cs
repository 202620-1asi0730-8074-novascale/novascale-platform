using NovaScale.Platform.API.LeadManagement.Domain.Model.Commands;
using NovaScale.Platform.API.LeadManagement.Interfaces.REST.Resources;

namespace NovaScale.Platform.API.LeadManagement.Interfaces.REST.Transform;

public static class UpdateTagCommandFromResourceAssembler
{
    public static UpdateTagCommand ToCommandFromResource(UpdateTagResource resource) => new(resource.Name, resource.Color);
}
