using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.Shared.Application.Services;
using NovaScale.Platform.API.Shared.Domain.Exceptions;
using NovaScale.Platform.API.Shared.Domain.Repositories;

namespace NovaScale.Platform.API.ConversationManagement.Application.Internal.Services;

public class NotificationLifecycle(IEntityRelations relations, EntityRules rules) : EntityLifecycle<Notification>
{
    public override Expression<Func<Notification, bool>> Scope(long userId, bool isOwner) => entity => isOwner || entity.UserId == userId;
    public override async Task BeforeSaveAsync(Notification entity, bool creating, CancellationToken cancellationToken)
    {
        await rules.RequireAssigneeAsync(entity.UserId, cancellationToken);
        var conversation = await relations.Query<Conversation>().SingleOrDefaultAsync(item => item.Id == entity.ConversationId, cancellationToken)
            ?? throw new BusinessException("InvalidReference");
        if (conversation.UserId != entity.UserId) throw new BusinessException("InvalidReference");
    }
}
