using NovaScale.Platform.API.ConversationManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.Commands;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.Queries;
using NovaScale.Platform.API.Shared.Application.Patterns;

namespace NovaScale.Platform.API.ConversationManagement.Application.Services;

public interface IMessageQueryService
{
    Task<Message> Handle(GetMessageByIdQuery query, CancellationToken cancellationToken = default);
    Task<PageResult<Message>> Handle(GetAllMessagesQuery query, CancellationToken cancellationToken = default);
}
