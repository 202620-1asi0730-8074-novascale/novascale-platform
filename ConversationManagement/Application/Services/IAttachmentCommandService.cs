using NovaScale.Platform.API.ConversationManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.Commands;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.Queries;

namespace NovaScale.Platform.API.ConversationManagement.Application.Services;

public interface IAttachmentCommandService
{
    Task<Attachment> Handle(CreateAttachmentCommand command, CancellationToken cancellationToken = default);
    Task<Attachment> Handle(long id, UpdateAttachmentCommand command, CancellationToken cancellationToken = default);
    Task RemoveAsync(long id, CancellationToken cancellationToken = default);
}
