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

public class RoleCommandService(IRoleRepository repository, IUnitOfWork unitOfWork, IRequestContext request,
    IAccessPolicy access, IEntityLifecycle<Role> lifecycle)
    : CrudCommandService<Role>(repository, unitOfWork, request, access, lifecycle), IRoleCommandService
{
    public Task<Role> Handle(CreateRoleCommand command, CancellationToken cancellationToken = default) =>
        CreateAsync(() => new Role(command), "manage_roles", cancellationToken);

    public Task<Role> Handle(long id, UpdateRoleCommand command, CancellationToken cancellationToken = default) =>
        UpdateAsync(id, entity => entity.Update(command), "manage_roles", cancellationToken);

    public Task RemoveAsync(long id, CancellationToken cancellationToken = default) =>
        DeleteAsync(id, "manage_roles", cancellationToken);
}
