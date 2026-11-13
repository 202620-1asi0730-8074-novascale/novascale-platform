using NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.LeadManagement.Domain.Model.Commands;
using NovaScale.Platform.API.LeadManagement.Domain.Model.Queries;
using NovaScale.Platform.API.LeadManagement.Application.Services;
using NovaScale.Platform.API.LeadManagement.Domain.Repositories;
using NovaScale.Platform.API.Shared.Application.Security;
using NovaScale.Platform.API.Shared.Application.Services;
using NovaScale.Platform.API.Shared.Domain.Repositories;
using NovaScale.Platform.API.Shared.Application.Internal.CommandServices;

namespace NovaScale.Platform.API.LeadManagement.Application.Internal.CommandServices;

public class OpportunityCommandService(IOpportunityRepository repository, IUnitOfWork unitOfWork, IRequestContext request,
    IAccessPolicy access, IEntityLifecycle<Opportunity> lifecycle)
    : CrudCommandService<Opportunity>(repository, unitOfWork, request, access, lifecycle), IOpportunityCommandService
{
    public Task<Opportunity> Handle(CreateOpportunityCommand command, CancellationToken cancellationToken = default) =>
        CreateAsync(() => new Opportunity(command), "write_contacts", cancellationToken);

    public Task<Opportunity> Handle(long id, UpdateOpportunityCommand command, CancellationToken cancellationToken = default) =>
        UpdateAsync(id, entity => entity.Update(command), "write_contacts", cancellationToken);

    public Task RemoveAsync(long id, CancellationToken cancellationToken = default) =>
        DeleteAsync(id, "delete_contacts", cancellationToken);
}
