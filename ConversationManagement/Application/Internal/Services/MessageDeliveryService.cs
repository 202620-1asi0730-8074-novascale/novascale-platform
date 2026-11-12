using NovaScale.Platform.API.ConversationManagement.Application.Services;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.Commands;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.ValueObjects;
using NovaScale.Platform.API.ConversationManagement.Domain.Repositories;
using NovaScale.Platform.API.Shared.Application.Security;
using NovaScale.Platform.API.Shared.Application.Services;
using NovaScale.Platform.API.Shared.Domain.Exceptions;
using NovaScale.Platform.API.Shared.Domain.Repositories;

namespace NovaScale.Platform.API.ConversationManagement.Application.Internal.Services;

public class MessageDeliveryService(IMessageRepository repository, IUnitOfWork unitOfWork, IRequestContext request,
    IAccessPolicy access, IEntityLifecycle<Message> lifecycle) : IMessageDeliveryService
{
    public async Task<Message> CreateAsync(CreateMessageCommand command, IReadOnlyList<MessageAttachment>? attachments, CancellationToken cancellationToken = default)
    {
        await access.RequireAsync("write_conversations", cancellationToken);
        return await unitOfWork.ExecuteAsync(async () =>
        {
            await access.RequireAsync("write_conversations", cancellationToken);
            var message = new Message(command) { OrganizationId = request.OrganizationId };
            message.SetAttachments(CreateAttachments(attachments, message.Id));
            await lifecycle.BeforeSaveAsync(message, true, cancellationToken);
            await repository.AddAsync(message, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            await lifecycle.AfterSaveAsync(message, true, cancellationToken);
            return message;
        }, cancellationToken);
    }

    public async Task<Message> UpdateAsync(long id, UpdateMessageCommand command, IReadOnlyList<MessageAttachment>? attachments, CancellationToken cancellationToken = default)
    {
        await access.RequireAsync("write_conversations", cancellationToken);
        return await unitOfWork.ExecuteAsync(async () =>
        {
            await access.RequireAsync("write_conversations", cancellationToken);
            var message = await repository.FindByIdAsync(id, cancellationToken) ?? throw new BusinessException("NotFound", 404);
            if (!lifecycle.Scope(request.UserId, await access.IsOwnerAsync(cancellationToken)).Compile()(message))
                throw new BusinessException("Forbidden", 403);
            message.Update(command);
            message.SetAttachments(CreateAttachments(attachments, message.Id));
            await lifecycle.BeforeSaveAsync(message, false, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            await lifecycle.AfterSaveAsync(message, false, cancellationToken);
            return message;
        }, cancellationToken);
    }

    private IReadOnlyList<Attachment> CreateAttachments(IReadOnlyList<MessageAttachment>? attachments, long messageId)
    {
        if (attachments is null) return [];
        if (attachments.Count > 3 || attachments.Any(attachment => attachment is null)) throw new BusinessException("InvalidAttachment");
        return attachments.Select(attachment => new Attachment(new CreateAttachmentCommand(attachment.Name, attachment.ContentType, attachment.Data, messageId))
            { OrganizationId = request.OrganizationId }).ToArray();
    }
}
