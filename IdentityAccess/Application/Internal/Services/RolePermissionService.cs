using Microsoft.EntityFrameworkCore;
using NovaScale.Platform.API.IdentityAccess.Application.Services;
using NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates;
using NovaScale.Platform.API.Shared.Application.Services;
using NovaScale.Platform.API.Shared.Application.Security;
using NovaScale.Platform.API.Shared.Domain.Exceptions;
using NovaScale.Platform.API.Shared.Domain.Repositories;

namespace NovaScale.Platform.API.IdentityAccess.Application.Internal.Services;

public class RolePermissionService(IEntityRelations relations, IUnitOfWork unitOfWork, IAccessPolicy access,
    IRequestContext request) : IRolePermissionService
{
    private async Task CheckAsync(long parentId, bool writing, CancellationToken cancellationToken)
    {
        await access.RequireAsync(writing ? "manage_roles" : "read_users", cancellationToken);

        var role = await relations.Query<Role>().SingleOrDefaultAsync(item => item.Id == parentId, cancellationToken)
            ?? throw new BusinessException("NotFound", 404);
        if (writing && role.SystemCode == "OWNER") throw new BusinessException("InUse", 409);

    }

    public async Task<IEnumerable<RolePermission>> ListAsync(long parentId, CancellationToken cancellationToken)
    {
        await CheckAsync(parentId, false, cancellationToken);
        return await relations.Query<RolePermission>().Where(item => item.RoleId == parentId).ToListAsync(cancellationToken);
    }

    public async Task<RolePermission> GetAsync(long parentId, long targetId, CancellationToken cancellationToken)
    {
        await CheckAsync(parentId, false, cancellationToken);
        return await relations.Query<RolePermission>().SingleOrDefaultAsync(item => item.RoleId == parentId && item.PermissionId == targetId, cancellationToken)
            ?? throw new BusinessException("NotFound", 404);
    }

    public async Task<RolePermission> AssignAsync(long parentId, long targetId, CancellationToken cancellationToken)
    {
        await CheckAsync(parentId, true, cancellationToken);
        if (!await relations.Query<Permission>().AnyAsync(item => item.Id == targetId, cancellationToken)) throw new BusinessException("InvalidReference");
        return await unitOfWork.ExecuteAsync(async () =>
        {
            var link = new RolePermission { OrganizationId = request.OrganizationId, RoleId = parentId, PermissionId = targetId };
            relations.Add(link);
            await Task.CompletedTask;
            return link;
        }, cancellationToken);
    }

    public async Task<RolePermission> ReplaceAsync(long parentId, long targetId, long replacementId, CancellationToken cancellationToken)
    {
        await CheckAsync(parentId, true, cancellationToken);
        if (!await relations.Query<Permission>().AnyAsync(item => item.Id == replacementId, cancellationToken)) throw new BusinessException("InvalidReference");
        return await unitOfWork.ExecuteAsync(async () =>
        {
            var old = await GetAsync(parentId, targetId, cancellationToken);
            if (replacementId == targetId) return old;
            relations.Remove(old);
            var link = new RolePermission { OrganizationId = request.OrganizationId, RoleId = parentId, PermissionId = replacementId };
            relations.Add(link);
            return link;
        }, cancellationToken);
    }

    public async Task RemoveAsync(long parentId, long targetId, CancellationToken cancellationToken)
    {
        await CheckAsync(parentId, true, cancellationToken);
        await unitOfWork.ExecuteAsync(async () =>
        {
            relations.Remove(await GetAsync(parentId, targetId, cancellationToken));
            return true;
        }, cancellationToken);
    }
}
