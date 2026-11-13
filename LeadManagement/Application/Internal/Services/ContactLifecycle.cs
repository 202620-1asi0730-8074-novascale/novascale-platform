using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.LeadManagement.Domain.Model.Commands;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.Shared.Application.Security;
using NovaScale.Platform.API.Shared.Application.Services;
using NovaScale.Platform.API.Shared.Domain.Exceptions;
using NovaScale.Platform.API.Shared.Domain.Repositories;

namespace NovaScale.Platform.API.LeadManagement.Application.Internal.Services;

public class ContactLifecycle(IEntityRelations relations, EntityRules rules, IRequestContext request) : EntityLifecycle<Contact>
{
    public override Expression<Func<Contact, bool>> Scope(long userId, bool isOwner) => entity => isOwner || entity.UserId == userId;

    public override async Task BeforeSaveAsync(Contact entity, bool creating, CancellationToken cancellationToken)
    {
        await rules.RequireAssigneeAsync(entity.UserId, cancellationToken);
        entity.ChangeStage(entity.ContactType == "CUSTOMER" ? "WON" : entity.LeadStatus);
    }

    public override Task AfterSaveAsync(Contact entity, bool creating, CancellationToken cancellationToken)
    {
        relations.Add(new Activity(new CreateActivityCommand(creating ? "CONTACT_CREATED" : "CONTACT_UPDATED", entity.FullName,
            request.UserId, entity.Id)) { OrganizationId = request.OrganizationId });
        return Task.CompletedTask;
    }

    public override async Task BeforeDeleteAsync(Contact entity, CancellationToken cancellationToken)
    {
        if (await relations.Query<Opportunity>().AnyAsync(item => item.ContactId == entity.Id, cancellationToken))
            throw new BusinessException("InUse", 409);
        var conversations = await relations.Query<Conversation>().Where(item => item.ContactId == entity.Id).ToListAsync(cancellationToken);
        foreach (var conversation in conversations)
        {
            foreach (var message in await relations.Query<Message>().Where(item => item.ConversationId == conversation.Id).ToListAsync(cancellationToken)) relations.Remove(message);
            foreach (var notification in await relations.Query<Notification>().Where(item => item.ConversationId == conversation.Id).ToListAsync(cancellationToken)) relations.Remove(notification);
            relations.Remove(conversation);
        }
        foreach (var link in await relations.Query<ContactTag>().Where(item => item.ContactId == entity.Id).ToListAsync(cancellationToken)) relations.Remove(link);
        foreach (var activity in await relations.Query<Activity>().Where(item => item.ContactId == entity.Id).ToListAsync(cancellationToken))
            activity.Update(new UpdateActivityCommand(activity.Type, activity.Content, activity.UserId, null));
    }
}
