using NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates;
using NovaScale.Platform.API.Shared.Application.Services;
using NovaScale.Platform.API.Shared.Domain.Exceptions;

namespace NovaScale.Platform.API.IdentityAccess.Application.Internal.Services;

public class RoleLifecycle : EntityLifecycle<Role>
{
    public override Task BeforeDeleteAsync(Role entity, CancellationToken cancellationToken)
    {
        if (entity.SystemCode.Length > 0) throw new BusinessException("InUse", 409);
        return Task.CompletedTask;
    }
}
