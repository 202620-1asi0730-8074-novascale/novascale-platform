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

public class TagQueryService(ITagRepository repository, IRequestContext request, IAccessPolicy access,
    IEntityLifecycle<Tag> lifecycle) : CrudQueryService<Tag>(repository, request, access, lifecycle), ITagQueryService
{
    public Task<Tag> Handle(GetTagByIdQuery query, CancellationToken cancellationToken = default) =>
        FindAsync(query.Id, "read_contacts", cancellationToken);

    public Task<PageResult<Tag>> Handle(GetAllTagsQuery query, CancellationToken cancellationToken = default)
    {
        var search = query.Search?.Trim() ?? string.Empty;
        return ListAsync(query.Page, query.PageSize, entity => search == "" || entity.Name.Contains(search) || entity.Color.Contains(search), "read_contacts", cancellationToken);
    }
}
