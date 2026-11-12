using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.Commands;
using NovaScale.Platform.API.Shared.Application.Services;
using NovaScale.Platform.API.Shared.Domain.Exceptions;
using NovaScale.Platform.API.Shared.Domain.Repositories;

namespace NovaScale.Platform.API.ConversationManagement.Application.Internal.Services;

public class MessageLifecycle(IEntityRelations relations, EntityRules rules, IConfiguration configuration) : EntityLifecycle<Message>
{
    public override Expression<Func<Message, bool>> Scope(long userId, bool isOwner) => entity => isOwner || entity.UserId == userId;

    public override async Task BeforeSaveAsync(Message entity, bool creating, CancellationToken cancellationToken)
    {
        var conversation = await relations.Query<Conversation>().SingleOrDefaultAsync(item => item.Id == entity.ConversationId, cancellationToken)
            ?? throw new BusinessException("InvalidReference");
        await rules.RequireAssigneeAsync(conversation.UserId, cancellationToken);
        if (conversation.Status != "OPEN") throw new BusinessException("ConversationClosed", 409);
        entity.AssignUser(conversation.UserId);
        if (!creating)
        {
            var original = await relations.Query<Message>().AsNoTracking().SingleAsync(item => item.Id == entity.Id, cancellationToken);
            if (original.ConversationId != entity.ConversationId || original.SenderType != entity.SenderType || original.ExternalMessageId != null)
                throw new BusinessException("InvalidReference");
        }
        if (entity.Content.Length == 0 && entity.Attachments.Count == 0) throw new BusinessException("InvalidData");
    }

    public override async Task AfterSaveAsync(Message entity, bool creating, CancellationToken cancellationToken)
    {
        await RefreshAsync(entity.ConversationId, null, cancellationToken);
        if (creating && entity.SenderType == "IN")
        {
            var conversation = await relations.Query<Conversation>().SingleAsync(item => item.Id == entity.ConversationId, cancellationToken);
            relations.Add(new Notification(new CreateNotificationCommand("NEW_MESSAGE", "New message", false, conversation.UserId, conversation.Id))
                { OrganizationId = entity.OrganizationId });
        }
    }

    public override async Task BeforeDeleteAsync(Message entity, CancellationToken cancellationToken)
    {
        if (entity.ExternalMessageId != null) throw new BusinessException("ExternalMessageReadOnly", 409);
        await RefreshAsync(entity.ConversationId, entity.Id, cancellationToken);
    }

    private async Task RefreshAsync(long conversationId, long? excludedId, CancellationToken cancellationToken)
    {
        var messages = relations.Query<Message>().Where(item => item.ConversationId == conversationId && item.Id != excludedId);
        var last = await messages.OrderByDescending(item => item.SentAt).ThenByDescending(item => item.Id).FirstOrDefaultAsync(cancellationToken);
        DateTime? pending = null;
        if (last?.SenderType == "IN")
        {
            var lastOutgoing = await messages.Where(item => item.SenderType == "OUT").Select(item => (DateTime?)item.SentAt).MaxAsync(cancellationToken);
            pending = await messages.Where(item => item.SenderType == "IN" && (!lastOutgoing.HasValue || item.SentAt > lastOutgoing))
                .Select(item => (DateTime?)item.SentAt).MinAsync(cancellationToken);
        }
        var conversation = await relations.Query<Conversation>().SingleAsync(item => item.Id == conversationId, cancellationToken);
        conversation.UpdateTimeline(pending, last?.SentAt, configuration.GetValue("Conversations:PriorityThresholdMinutes", 120));
    }
}
