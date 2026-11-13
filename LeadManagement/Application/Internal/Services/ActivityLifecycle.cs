using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.Shared.Application.Services;
using NovaScale.Platform.API.Shared.Application.Security;
using NovaScale.Platform.API.Shared.Domain.Exceptions;
using NovaScale.Platform.API.Shared.Domain.Repositories;

namespace NovaScale.Platform.API.LeadManagement.Application.Internal.Services;

public class ActivityLifecycle(IEntityRelations relations, EntityRules rules) : EntityLifecycle<Activity>
{
    public override Expression<Func<Activity, bool>> Scope(long userId, bool isOwner) => entity => isOwner || entity.UserId == userId;
    public override async Task BeforeSaveAsync(Activity entity, bool creating, CancellationToken cancellationToken)
    {
        await rules.RequireAssigneeAsync(entity.UserId, cancellationToken);
        if (entity.ContactId.HasValue)
        {
            var contact = await relations.Query<Contact>().SingleOrDefaultAsync(item => item.Id == entity.ContactId, cancellationToken)
                ?? throw new BusinessException("InvalidReference");
            await rules.RequireAssigneeAsync(contact.UserId, cancellationToken);
        }
    }
}
