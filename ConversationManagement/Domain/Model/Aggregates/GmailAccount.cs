using NovaScale.Platform.API.Shared.Domain.Model;

namespace NovaScale.Platform.API.ConversationManagement.Domain.Model.Aggregates;

public sealed class GmailAccount : Entity
{
    public long ConnectedByUserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string ProtectedRefreshToken { get; set; } = string.Empty;
    public string ProtectedAccessToken { get; set; } = string.Empty;
    public DateTime AccessTokenExpiresAt { get; set; }
}
