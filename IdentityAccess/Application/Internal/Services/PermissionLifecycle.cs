using NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates;
using NovaScale.Platform.API.Shared.Application.Services;
using NovaScale.Platform.API.Shared.Domain.Exceptions;

namespace NovaScale.Platform.API.IdentityAccess.Application.Internal.Services;

public class PermissionLifecycle : EntityLifecycle<Permission>
{
    public override Task BeforeSaveAsync(Permission entity, bool creating, CancellationToken cancellationToken)
    {
        if (!System.Text.RegularExpressions.Regex.IsMatch(entity.Code, "^[a-z][a-z0-9_]{1,79}$"))
            throw new BusinessException("InvalidData");
        return Task.CompletedTask;
    }
}
