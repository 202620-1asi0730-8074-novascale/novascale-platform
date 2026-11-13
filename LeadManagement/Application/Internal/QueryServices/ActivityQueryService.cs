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

public class ActivityQueryService(IActivityRepository repository, IRequestContext request, IAccessPolicy access,
    IEntityLifecycle<Activity> lifecycle) : CrudQueryService<Activity>(repository, request, access, lifecycle), IActivityQueryService
{
    public Task<Activity> Handle(GetActivityByIdQuery query, CancellationToken cancellationToken = default) =>
        FindAsync(query.Id, "read_contacts", cancellationToken);

    public Task<PageResult<Activity>> Handle(GetAllActivitiesQuery query, CancellationToken cancellationToken = default)
    {
        var search = query.Search?.Trim() ?? string.Empty;
        return ListAsync(query.Page, query.PageSize, entity => search == "" || entity.Type.Contains(search) || entity.Content.Contains(search), "read_contacts", cancellationToken);
    }
}
