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

public class UserQueryService(IUserRepository repository, IRequestContext request, IAccessPolicy access,
    IEntityLifecycle<User> lifecycle) : CrudQueryService<User>(repository, request, access, lifecycle), IUserQueryService
{
    public Task<User> Handle(GetUserByIdQuery query, CancellationToken cancellationToken = default) =>
        FindAsync(query.Id, "read_users", cancellationToken);

    public Task<PageResult<User>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken = default)
    {
        var search = query.Search?.Trim() ?? string.Empty;
        return ListAsync(query.Page, query.PageSize, entity => search == "" || entity.FullName.Contains(search) || entity.Email.Contains(search) || entity.Phone.Contains(search) || entity.Status.Contains(search), "read_users", cancellationToken);
    }
}
