using NovaScale.Platform.API.LeadManagement.Application.Services;
using NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.LeadManagement.Interfaces.REST.Resources;

namespace NovaScale.Platform.API.LeadManagement.Interfaces.REST.Transform;

public static class ContactResourceFromEntityAssembler
{
    public static ContactResource ToResourceFromEntity(Contact entity) => new(entity.Id, entity.FullName, entity.Email, entity.Phone ?? string.Empty, entity.Company, entity.ContactType, entity.LeadStatus, entity.Source, entity.UserId, entity.CreatedAt, entity.UpdatedAt);
    public static ContactResource ToResourceFromDetails(ContactDetails details) => ToResourceFromEntity(details.Contact) with
    {
        EstimatedAmount = details.EstimatedAmount, OpportunityId = details.OpportunityId, Tags = details.Tags,
        Owner = details.Owner, WonAt = details.WonAt
    };
}
