using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.Shared.Application.Services;
using NovaScale.Platform.API.Shared.Domain.Exceptions;
using NovaScale.Platform.API.Shared.Domain.Repositories;

namespace NovaScale.Platform.API.ConversationManagement.Application.Internal.Services;

public class AttachmentLifecycle(IEntityRelations relations, EntityRules rules) : EntityLifecycle<Attachment>
{
    public override Expression<Func<Attachment, bool>> Scope(long userId, bool isOwner)
    {
        var messages = relations.Query<Message>();
        return entity => isOwner || messages.Any(message => message.Id == entity.MessageId && message.UserId == userId);
    }

    public override async Task BeforeSaveAsync(Attachment entity, bool creating, CancellationToken cancellationToken)
    {
        await RequireMessageAsync(entity.MessageId, cancellationToken);
        if (!creating)
        {
            var original = await relations.Query<Attachment>().AsNoTracking().SingleAsync(item => item.Id == entity.Id, cancellationToken);
            if (original.MessageId != entity.MessageId) throw new BusinessException("InvalidReference");
        }
        if (await relations.Query<Attachment>().CountAsync(item => item.MessageId == entity.MessageId && item.Id != entity.Id, cancellationToken) >= 3)
            throw new BusinessException("InvalidAttachment");
    }

    public override async Task BeforeDeleteAsync(Attachment entity, CancellationToken cancellationToken)
    {
        var message = await RequireMessageAsync(entity.MessageId, cancellationToken);
        if (message.Content.Length == 0 && !await relations.Query<Attachment>().AnyAsync(item => item.MessageId == entity.MessageId && item.Id != entity.Id, cancellationToken))
            throw new BusinessException("InvalidData", 409);
    }

    private async Task<Message> RequireMessageAsync(long id, CancellationToken cancellationToken)
    {
        var message = await relations.Query<Message>().SingleOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new BusinessException("InvalidReference");
        await rules.RequireAssigneeAsync(message.UserId, cancellationToken);
        if (message.ExternalMessageId is not null) throw new BusinessException("ExternalMessageReadOnly", 409);
        var conversation = await relations.Query<Conversation>().SingleAsync(item => item.Id == message.ConversationId, cancellationToken);
        if (conversation.Status != "OPEN") throw new BusinessException("ConversationClosed", 409);
        return message;
    }
}
