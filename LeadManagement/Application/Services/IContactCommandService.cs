using NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.LeadManagement.Domain.Model.Commands;
using NovaScale.Platform.API.LeadManagement.Domain.Model.Queries;

namespace NovaScale.Platform.API.LeadManagement.Application.Services;

public interface IContactCommandService
{
    Task<Contact> Handle(CreateContactCommand command, CancellationToken cancellationToken = default);
    Task<Contact> Handle(long id, UpdateContactCommand command, CancellationToken cancellationToken = default);
    Task RemoveAsync(long id, CancellationToken cancellationToken = default);
}
