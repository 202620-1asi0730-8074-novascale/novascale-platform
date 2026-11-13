using NovaScale.Platform.API.Shared.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.Commands;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.Queries;
using NovaScale.Platform.API.ConversationManagement.Application.Services;
using NovaScale.Platform.API.ConversationManagement.Domain.Repositories;
using NovaScale.Platform.API.Shared.Application.Security;
using NovaScale.Platform.API.Shared.Application.Services;
using NovaScale.Platform.API.Shared.Domain.Repositories;
using NovaScale.Platform.API.Shared.Application.Internal.QueryServices;
using NovaScale.Platform.API.Shared.Application.Patterns;

namespace NovaScale.Platform.API.ConversationManagement.Application.Internal.QueryServices;

public class ConversationQueryService(IConversationRepository repository, IRequestContext request, IAccessPolicy access,
    IEntityLifecycle<Conversation> lifecycle, IEntityRelations relations, IConfiguration configuration) : CrudQueryService<Conversation>(repository, request, access, lifecycle), IConversationQueryService
{
    public async Task<Conversation> Handle(GetConversationByIdQuery query, CancellationToken cancellationToken = default)
    {
        var entity = await FindAsync(query.Id, "read_conversations", cancellationToken);
        entity.UpdateTimeline(entity.PendingSince, entity.LastMessageAt, configuration.GetValue("Conversations:PriorityThresholdMinutes", 120));
        return entity;
    }

    public async Task<PageResult<Conversation>> Handle(GetAllConversationsQuery query, CancellationToken cancellationToken = default)
    {
        var search = query.Search?.Trim() ?? string.Empty;
        if (query.Status is not null && !new[] { "OPEN", "CLOSED" }.Contains(query.Status)) throw new BusinessException("InvalidState");
        if (query.Priority is not null && !new[] { "NORMAL", "HIGH" }.Contains(query.Priority)) throw new BusinessException("InvalidState");
        var links = relations.Query<ContactTag>();
        var cutoff = DateTime.UtcNow.AddMinutes(-configuration.GetValue("Conversations:PriorityThresholdMinutes", 120));
        var result = await ListAsync(query.Page, query.PageSize, entity => (search == "" || entity.Channel.Contains(search) || entity.Status.Contains(search)) && (string.IsNullOrEmpty(query.Status) || entity.Status == query.Status) && (!query.UserId.HasValue || entity.UserId == query.UserId) && (string.IsNullOrEmpty(query.Priority) || (query.Priority == "HIGH" ? entity.PendingSince != null && entity.PendingSince <= cutoff : entity.PendingSince == null || entity.PendingSince > cutoff)) && (!query.ContactId.HasValue || entity.ContactId == query.ContactId) && (!query.TagId.HasValue || links.Any(link => link.ContactId == entity.ContactId && link.TagId == query.TagId)), "read_conversations", cancellationToken);
        foreach (var entity in result.Items) entity.UpdateTimeline(entity.PendingSince, entity.LastMessageAt, configuration.GetValue("Conversations:PriorityThresholdMinutes", 120));
        return result;
    }
}
