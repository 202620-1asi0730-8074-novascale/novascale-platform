using NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates;
using NovaScale.Platform.API.IdentityAccess.Domain.Model.Commands;
using NovaScale.Platform.API.IdentityAccess.Domain.Model.Queries;

namespace NovaScale.Platform.API.IdentityAccess.Application.Services;

public interface IPermissionCommandService
{
    Task<Permission> Handle(CreatePermissionCommand command, CancellationToken cancellationToken = default);
    Task<Permission> Handle(long id, UpdatePermissionCommand command, CancellationToken cancellationToken = default);
    Task RemoveAsync(long id, CancellationToken cancellationToken = default);
}
