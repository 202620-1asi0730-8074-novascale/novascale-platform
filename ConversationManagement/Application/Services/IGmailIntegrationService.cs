using NovaScale.Platform.API.ConversationManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.ValueObjects;

namespace NovaScale.Platform.API.ConversationManagement.Application.Services;

public record GmailStatus(bool Configured, bool Connected, string? Email, bool CanManage);
public record GmailSendRequest(Guid RequestId, string Content, IReadOnlyList<MessageAttachment>? Attachments);

public interface IGmailIntegrationService
{
    Task<GmailStatus> StatusAsync(CancellationToken cancellationToken);
    Task<string> ConnectAsync(CancellationToken cancellationToken);
    Task CompleteConnectionAsync(string? code, string? state, CancellationToken cancellationToken);
    string ReturnUrl(bool connected);
    Task DisconnectAsync(CancellationToken cancellationToken);
    Task<Message> SendAsync(long contactId, GmailSendRequest resource, CancellationToken cancellationToken);
    Task<int> SyncAsync(long contactId, CancellationToken cancellationToken);
}
