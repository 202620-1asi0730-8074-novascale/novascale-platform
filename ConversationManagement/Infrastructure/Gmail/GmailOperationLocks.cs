using System.Collections.Concurrent;

namespace NovaScale.Platform.API.ConversationManagement.Infrastructure.Gmail;

public sealed class GmailOperationLocks
{
    private readonly ConcurrentDictionary<long, SemaphoreSlim> gates = new();
    public SemaphoreSlim ForOrganization(long organizationId) => gates.GetOrAdd(organizationId, _ => new SemaphoreSlim(1, 1));
}
