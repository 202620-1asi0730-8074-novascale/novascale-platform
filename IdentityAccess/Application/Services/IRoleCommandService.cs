using NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates;
using NovaScale.Platform.API.IdentityAccess.Domain.Model.Commands;
using NovaScale.Platform.API.IdentityAccess.Domain.Model.Queries;

namespace NovaScale.Platform.API.IdentityAccess.Application.Services;

public interface IRoleCommandService
{
    Task<Role> Handle(CreateRoleCommand command, CancellationToken cancellationToken = default);
    Task<Role> Handle(long id, UpdateRoleCommand command, CancellationToken cancellationToken = default);
    Task RemoveAsync(long id, CancellationToken cancellationToken = default);
}
