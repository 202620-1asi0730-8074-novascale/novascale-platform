using Microsoft.EntityFrameworkCore;
using NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates;
using NovaScale.Platform.API.Shared.Application.Security;
using NovaScale.Platform.API.Shared.Domain.Exceptions;
using NovaScale.Platform.API.Shared.Infrastructure.Persistence.EFC.Configuration;

namespace NovaScale.Platform.API.IdentityAccess.Infrastructure.Security;

public class AccessPolicy(AppDbContext context, IRequestContext request) : IAccessPolicy
{
    public async Task<bool> IsOwnerAsync(CancellationToken cancellationToken = default) =>
        await (from user in context.Set<User>() join role in context.Set<Role>() on user.RoleId equals role.Id
            where user.Id == request.UserId && user.Status == "ACTIVE" && role.SystemCode == "OWNER"
            select user.Id).AnyAsync(cancellationToken);

    public async Task RequireAsync(string permission, CancellationToken cancellationToken = default)
    {
        var user = await context.Set<User>().SingleOrDefaultAsync(user => user.Id == request.UserId, cancellationToken);
        if (user is null || user.Status != "ACTIVE") throw new BusinessException("Unauthorized", 401);
        var owner = await IsOwnerAsync(cancellationToken);
        if (permission.StartsWith("manage_") && permission != "manage_tags" && !owner)
            throw new BusinessException("Forbidden", 403);
        if ((permission == "delete_contacts" && !user.CanDeleteLeads) || (permission == "export_data" && !user.CanExportData))
            throw new BusinessException("Forbidden", 403);
        if (owner) return;
        var allowed = await (from link in context.Set<RolePermission>()
            join item in context.Set<Permission>() on link.PermissionId equals item.Id
            where link.RoleId == user.RoleId && item.Code == permission select item.Id).AnyAsync(cancellationToken);
        if (!allowed) throw new BusinessException("Forbidden", 403);
    }
}
