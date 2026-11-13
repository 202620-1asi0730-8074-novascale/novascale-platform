using NovaScale.Platform.API.Shared.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.LeadManagement.Domain.Model.Commands;
using NovaScale.Platform.API.LeadManagement.Domain.Model.Queries;
using NovaScale.Platform.API.LeadManagement.Application.Services;
using NovaScale.Platform.API.LeadManagement.Domain.Repositories;
using NovaScale.Platform.API.Shared.Application.Security;
using NovaScale.Platform.API.Shared.Application.Services;
using NovaScale.Platform.API.Shared.Domain.Repositories;
using NovaScale.Platform.API.Shared.Application.Internal.QueryServices;
using NovaScale.Platform.API.Shared.Application.Patterns;

namespace NovaScale.Platform.API.LeadManagement.Application.Internal.QueryServices;

public class OpportunityQueryService(IOpportunityRepository repository, IRequestContext request, IAccessPolicy access,
    IEntityLifecycle<Opportunity> lifecycle) : CrudQueryService<Opportunity>(repository, request, access, lifecycle), IOpportunityQueryService
{
    public Task<Opportunity> Handle(GetOpportunityByIdQuery query, CancellationToken cancellationToken = default) =>
        FindAsync(query.Id, "read_contacts", cancellationToken);

    public Task<PageResult<Opportunity>> Handle(GetAllOpportunitiesQuery query, CancellationToken cancellationToken = default)
    {
        var search = query.Search?.Trim() ?? string.Empty;
        if (query.Status is not null && !new[] { "NEW", "CONTACTED", "PROPOSAL", "WON", "LOST" }.Contains(query.Status)) throw new BusinessException("InvalidState");
        return ListAsync(query.Page, query.PageSize, entity => (search == "" || entity.Title.Contains(search) || entity.Status.Contains(search)) && (string.IsNullOrEmpty(query.Status) || entity.Status == query.Status) && (!query.UserId.HasValue || entity.UserId == query.UserId) && (!query.ContactId.HasValue || entity.ContactId == query.ContactId), "read_contacts", cancellationToken);
    }
}
