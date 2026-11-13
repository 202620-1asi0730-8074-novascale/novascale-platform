using NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.LeadManagement.Domain.Model.Commands;
using NovaScale.Platform.API.LeadManagement.Domain.Model.Queries;
using NovaScale.Platform.API.Shared.Application.Patterns;

namespace NovaScale.Platform.API.LeadManagement.Application.Services;

public record ContactDetails(Contact Contact, decimal EstimatedAmount, long? OpportunityId, IReadOnlyList<string> Tags, string Owner, DateTime? WonAt);

public interface IContactQueryService
{
    Task<Contact> Handle(GetContactByIdQuery query, CancellationToken cancellationToken = default);
    Task<PageResult<Contact>> Handle(GetAllContactsQuery query, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ContactDetails>> GetDetailsAsync(IReadOnlyList<Contact> contacts, CancellationToken cancellationToken = default);
}
