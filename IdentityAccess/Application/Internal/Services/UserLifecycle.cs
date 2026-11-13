using Microsoft.EntityFrameworkCore;
using NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates;
using NovaScale.Platform.API.Shared.Application.Services;
using NovaScale.Platform.API.Shared.Domain.Exceptions;
using NovaScale.Platform.API.Shared.Domain.Repositories;

namespace NovaScale.Platform.API.IdentityAccess.Application.Internal.Services;

public class UserLifecycle(IEntityRelations relations) : EntityLifecycle<User>
{
    public override async Task BeforeSaveAsync(User entity, bool creating, CancellationToken cancellationToken)
    {
        var role = await relations.Query<Role>().SingleOrDefaultAsync(role => role.Id == entity.RoleId, cancellationToken)
            ?? throw new BusinessException("InvalidReference");
        if (entity.PasswordHash.Length == 0) throw new BusinessException("InvalidPassword");
        if (!creating && (role.SystemCode != "OWNER" || entity.Status != "ACTIVE"))
            await RequireAnotherOwnerAsync(entity.Id, cancellationToken);
    }

    public override Task BeforeDeleteAsync(User entity, CancellationToken cancellationToken) => RequireAnotherOwnerAsync(entity.Id, cancellationToken);

    private async Task RequireAnotherOwnerAsync(long id, CancellationToken cancellationToken)
    {
        var ownerExists = await (from user in relations.Query<User>() join role in relations.Query<Role>() on user.RoleId equals role.Id
            where user.Id != id && user.Status == "ACTIVE" && role.SystemCode == "OWNER" select user.Id).AnyAsync(cancellationToken);
        if (!ownerExists) throw new BusinessException("LastOwner", 409);
    }
}
