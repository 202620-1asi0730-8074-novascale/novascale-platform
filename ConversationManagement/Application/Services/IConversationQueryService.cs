using NovaScale.Platform.API.ConversationManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.Commands;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.Queries;
using NovaScale.Platform.API.Shared.Application.Patterns;

namespace NovaScale.Platform.API.ConversationManagement.Application.Services;

public interface IConversationQueryService
{
    Task<Conversation> Handle(GetConversationByIdQuery query, CancellationToken cancellationToken = default);
    Task<PageResult<Conversation>> Handle(GetAllConversationsQuery query, CancellationToken cancellationToken = default);
}
