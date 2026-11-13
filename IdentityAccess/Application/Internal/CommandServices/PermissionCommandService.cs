using NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates;
using NovaScale.Platform.API.IdentityAccess.Domain.Model.Commands;
using NovaScale.Platform.API.IdentityAccess.Domain.Model.Queries;
using NovaScale.Platform.API.IdentityAccess.Application.Services;
using NovaScale.Platform.API.IdentityAccess.Domain.Repositories;
using NovaScale.Platform.API.Shared.Application.Security;
using NovaScale.Platform.API.Shared.Application.Services;
using NovaScale.Platform.API.Shared.Domain.Repositories;
using NovaScale.Platform.API.Shared.Application.Internal.CommandServices;

namespace NovaScale.Platform.API.IdentityAccess.Application.Internal.CommandServices;

public class PermissionCommandService(IPermissionRepository repository, IUnitOfWork unitOfWork, IRequestContext request,
    IAccessPolicy access, IEntityLifecycle<Permission> lifecycle)
    : CrudCommandService<Permission>(repository, unitOfWork, request, access, lifecycle), IPermissionCommandService
{
    public Task<Permission> Handle(CreatePermissionCommand command, CancellationToken cancellationToken = default) =>
        CreateAsync(() => new Permission(command), "manage_permissions", cancellationToken);

    public Task<Permission> Handle(long id, UpdatePermissionCommand command, CancellationToken cancellationToken = default) =>
        UpdateAsync(id, entity => entity.Update(command), "manage_permissions", cancellationToken);

    public Task RemoveAsync(long id, CancellationToken cancellationToken = default) =>
        DeleteAsync(id, "manage_permissions", cancellationToken);
}
