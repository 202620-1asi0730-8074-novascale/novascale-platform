using NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates;

namespace NovaScale.Platform.API.LeadManagement.Application.Services;

public interface IContactTagService
{
    Task<IEnumerable<ContactTag>> ListAsync(long parentId, CancellationToken cancellationToken);
    Task<ContactTag> GetAsync(long parentId, long targetId, CancellationToken cancellationToken);
    Task<ContactTag> AssignAsync(long parentId, long targetId, CancellationToken cancellationToken);
    Task<ContactTag> ReplaceAsync(long parentId, long targetId, long replacementId, CancellationToken cancellationToken);
    Task RemoveAsync(long parentId, long targetId, CancellationToken cancellationToken);
}
