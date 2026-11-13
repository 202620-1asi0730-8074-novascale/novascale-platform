using NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.LeadManagement.Domain.Model.Commands;
using NovaScale.Platform.API.LeadManagement.Domain.Model.Queries;

namespace NovaScale.Platform.API.LeadManagement.Application.Services;

public interface IOpportunityCommandService
{
    Task<Opportunity> Handle(CreateOpportunityCommand command, CancellationToken cancellationToken = default);
    Task<Opportunity> Handle(long id, UpdateOpportunityCommand command, CancellationToken cancellationToken = default);
    Task RemoveAsync(long id, CancellationToken cancellationToken = default);
}
