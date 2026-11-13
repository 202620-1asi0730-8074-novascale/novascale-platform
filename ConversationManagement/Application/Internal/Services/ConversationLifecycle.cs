using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.Shared.Application.Services;
using NovaScale.Platform.API.Shared.Domain.Exceptions;
using NovaScale.Platform.API.Shared.Domain.Repositories;

namespace NovaScale.Platform.API.ConversationManagement.Application.Internal.Services;

public class ConversationLifecycle(IEntityRelations relations, EntityRules rules, IConfiguration configuration) : EntityLifecycle<Conversation>
{
    public override Expression<Func<Conversation, bool>> Scope(long userId, bool isOwner) => entity => isOwner || entity.UserId == userId;
    public override async Task BeforeSaveAsync(Conversation entity, bool creating, CancellationToken cancellationToken)
    {
        await rules.RequireAssigneeAsync(entity.UserId, cancellationToken);
        var contact = await relations.Query<Contact>().SingleOrDefaultAsync(item => item.Id == entity.ContactId, cancellationToken)
            ?? throw new BusinessException("InvalidReference");
        await rules.RequireAssigneeAsync(contact.UserId, cancellationToken);
        if (!creating)
        {
            var original = await relations.Query<Conversation>().AsNoTracking().SingleAsync(item => item.Id == entity.Id, cancellationToken);
            if (original.ContactId != entity.ContactId) throw new BusinessException("InvalidReference");
            var messages = await relations.Query<Message>().Where(item => item.ConversationId == entity.Id).ToListAsync(cancellationToken);
            foreach (var message in messages) message.AssignUser(entity.UserId);
        }
        entity.UpdateTimeline(entity.PendingSince, entity.LastMessageAt, configuration.GetValue("Conversations:PriorityThresholdMinutes", 120));
    }

    public override async Task BeforeDeleteAsync(Conversation entity, CancellationToken cancellationToken)
    {
        foreach (var message in await relations.Query<Message>().Where(item => item.ConversationId == entity.Id).ToListAsync(cancellationToken)) relations.Remove(message);
        foreach (var notification in await relations.Query<Notification>().Where(item => item.ConversationId == entity.Id).ToListAsync(cancellationToken)) relations.Remove(notification);
    }
}
