using System.Collections.Concurrent;

namespace NovaScale.Platform.API.ConversationManagement.Infrastructure.Gmail;

public record GmailAuthorization(long OrganizationId, long UserId, string Verifier, DateTime ExpiresAt);

public sealed class GmailAuthorizationStore
{
    private readonly ConcurrentDictionary<string, GmailAuthorization> states = new();
    public void Add(string state, GmailAuthorization authorization)
    {
        foreach (var expired in states.Where(pair => pair.Value.ExpiresAt < DateTime.UtcNow)) states.TryRemove(expired.Key, out _);
        states[state] = authorization;
    }
    public GmailAuthorization? Consume(string state) => states.TryRemove(state, out var authorization)
        && authorization.ExpiresAt > DateTime.UtcNow ? authorization : null;
}
