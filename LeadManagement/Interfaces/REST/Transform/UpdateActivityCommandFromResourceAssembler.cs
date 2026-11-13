using NovaScale.Platform.API.LeadManagement.Domain.Model.Commands;
using NovaScale.Platform.API.LeadManagement.Interfaces.REST.Resources;

namespace NovaScale.Platform.API.LeadManagement.Interfaces.REST.Transform;

public static class UpdateActivityCommandFromResourceAssembler
{
    public static UpdateActivityCommand ToCommandFromResource(UpdateActivityResource resource) => new(resource.Type, resource.Content, resource.UserId, resource.ContactId);
}
