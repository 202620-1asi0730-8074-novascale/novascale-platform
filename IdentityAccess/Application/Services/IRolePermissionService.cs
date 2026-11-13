using NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates;

namespace NovaScale.Platform.API.IdentityAccess.Application.Services;

public interface IRolePermissionService
{
    Task<IEnumerable<RolePermission>> ListAsync(long parentId, CancellationToken cancellationToken);
    Task<RolePermission> GetAsync(long parentId, long targetId, CancellationToken cancellationToken);
    Task<RolePermission> AssignAsync(long parentId, long targetId, CancellationToken cancellationToken);
    Task<RolePermission> ReplaceAsync(long parentId, long targetId, long replacementId, CancellationToken cancellationToken);
    Task RemoveAsync(long parentId, long targetId, CancellationToken cancellationToken);
}
