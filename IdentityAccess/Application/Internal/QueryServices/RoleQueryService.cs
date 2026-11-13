using NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates;
using NovaScale.Platform.API.IdentityAccess.Domain.Model.Commands;
using NovaScale.Platform.API.IdentityAccess.Domain.Model.Queries;
using NovaScale.Platform.API.IdentityAccess.Application.Services;
using NovaScale.Platform.API.IdentityAccess.Domain.Repositories;
using NovaScale.Platform.API.Shared.Application.Security;
using NovaScale.Platform.API.Shared.Application.Services;
using NovaScale.Platform.API.Shared.Domain.Repositories;
using NovaScale.Platform.API.Shared.Application.Internal.QueryServices;
using NovaScale.Platform.API.Shared.Application.Patterns;

namespace NovaScale.Platform.API.IdentityAccess.Application.Internal.QueryServices;

public class RoleQueryService(IRoleRepository repository, IRequestContext request, IAccessPolicy access,
    IEntityLifecycle<Role> lifecycle) : CrudQueryService<Role>(repository, request, access, lifecycle), IRoleQueryService
{
    public Task<Role> Handle(GetRoleByIdQuery query, CancellationToken cancellationToken = default) =>
        FindAsync(query.Id, "read_users", cancellationToken);

    public Task<PageResult<Role>> Handle(GetAllRolesQuery query, CancellationToken cancellationToken = default)
    {
        var search = query.Search?.Trim() ?? string.Empty;
        return ListAsync(query.Page, query.PageSize, entity => search == "" || entity.Name.Contains(search) || entity.Description.Contains(search), "read_users", cancellationToken);
    }
}
