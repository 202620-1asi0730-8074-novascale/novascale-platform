using NovaScale.Platform.API.ConversationManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.Commands;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.Queries;
using NovaScale.Platform.API.Shared.Application.Patterns;

namespace NovaScale.Platform.API.ConversationManagement.Application.Services;

public interface IAttachmentQueryService
{
    Task<Attachment> Handle(GetAttachmentByIdQuery query, CancellationToken cancellationToken = default);
    Task<PageResult<Attachment>> Handle(GetAllAttachmentsQuery query, CancellationToken cancellationToken = default);
}
