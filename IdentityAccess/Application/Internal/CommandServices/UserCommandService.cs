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

public class UserCommandService(IUserRepository repository, IUnitOfWork unitOfWork, IRequestContext request,
    IAccessPolicy access, IEntityLifecycle<User> lifecycle)
    : CrudCommandService<User>(repository, unitOfWork, request, access, lifecycle), IUserCommandService
{
    public Task<User> Handle(CreateUserCommand command, CancellationToken cancellationToken = default) =>
        CreateAsync(() => new User(command), "manage_users", cancellationToken);

    public Task<User> Handle(long id, UpdateUserCommand command, CancellationToken cancellationToken = default) =>
        UpdateAsync(id, entity => entity.Update(command), "manage_users", cancellationToken);

    public Task RemoveAsync(long id, CancellationToken cancellationToken = default) =>
        DeleteAsync(id, "manage_users", cancellationToken);
}
