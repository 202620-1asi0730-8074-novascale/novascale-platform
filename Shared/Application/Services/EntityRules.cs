using Microsoft.EntityFrameworkCore;
using NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates;
using NovaScale.Platform.API.Shared.Application.Security;
using NovaScale.Platform.API.Shared.Domain.Exceptions;
using NovaScale.Platform.API.Shared.Domain.Repositories;

namespace NovaScale.Platform.API.Shared.Application.Services;

public class EntityRules(IEntityRelations relations, IRequestContext request, IAccessPolicy access)
{
    public async Task RequireAssigneeAsync(long userId, CancellationToken cancellationToken)
    {
        if (userId != request.UserId && !await access.IsOwnerAsync(cancellationToken))
            throw new BusinessException("Forbidden", 403);
        if (!await relations.Query<User>().AnyAsync(user => user.Id == userId && user.Status == "ACTIVE", cancellationToken))
            throw new BusinessException("InvalidReference");
    }
}
