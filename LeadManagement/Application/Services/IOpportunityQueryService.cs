using NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.LeadManagement.Domain.Model.Commands;
using NovaScale.Platform.API.LeadManagement.Domain.Model.Queries;
using NovaScale.Platform.API.Shared.Application.Patterns;

namespace NovaScale.Platform.API.LeadManagement.Application.Services;

public interface IOpportunityQueryService
{
    Task<Opportunity> Handle(GetOpportunityByIdQuery query, CancellationToken cancellationToken = default);
    Task<PageResult<Opportunity>> Handle(GetAllOpportunitiesQuery query, CancellationToken cancellationToken = default);
}
