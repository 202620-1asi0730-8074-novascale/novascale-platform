using Microsoft.EntityFrameworkCore;
using NovaScale.Platform.API.LeadManagement.Application.Services;
using NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.Shared.Application.Services;
using NovaScale.Platform.API.Shared.Application.Security;
using NovaScale.Platform.API.Shared.Domain.Exceptions;
using NovaScale.Platform.API.Shared.Domain.Repositories;

namespace NovaScale.Platform.API.LeadManagement.Application.Internal.Services;

public class ContactTagService(IEntityRelations relations, IUnitOfWork unitOfWork, IAccessPolicy access,
    IRequestContext request, EntityRules rules) : IContactTagService
{
    private async Task CheckAsync(long parentId, bool writing, CancellationToken cancellationToken)
    {
        await access.RequireAsync(writing ? "manage_tags" : "read_contacts", cancellationToken);

        var contact = await relations.Query<Contact>().SingleOrDefaultAsync(item => item.Id == parentId, cancellationToken)
            ?? throw new BusinessException("NotFound", 404);
        await rules.RequireAssigneeAsync(contact.UserId, cancellationToken);

    }

    public async Task<IEnumerable<ContactTag>> ListAsync(long parentId, CancellationToken cancellationToken)
    {
        await CheckAsync(parentId, false, cancellationToken);
        return await relations.Query<ContactTag>().Where(item => item.ContactId == parentId).ToListAsync(cancellationToken);
    }

    public async Task<ContactTag> GetAsync(long parentId, long targetId, CancellationToken cancellationToken)
    {
        await CheckAsync(parentId, false, cancellationToken);
        return await relations.Query<ContactTag>().SingleOrDefaultAsync(item => item.ContactId == parentId && item.TagId == targetId, cancellationToken)
            ?? throw new BusinessException("NotFound", 404);
    }

    public async Task<ContactTag> AssignAsync(long parentId, long targetId, CancellationToken cancellationToken)
    {
        await CheckAsync(parentId, true, cancellationToken);
        if (!await relations.Query<Tag>().AnyAsync(item => item.Id == targetId, cancellationToken)) throw new BusinessException("InvalidReference");
        return await unitOfWork.ExecuteAsync(async () =>
        {
            var link = new ContactTag { OrganizationId = request.OrganizationId, ContactId = parentId, TagId = targetId };
            relations.Add(link);
            await Task.CompletedTask;
            return link;
        }, cancellationToken);
    }

    public async Task<ContactTag> ReplaceAsync(long parentId, long targetId, long replacementId, CancellationToken cancellationToken)
    {
        await CheckAsync(parentId, true, cancellationToken);
        if (!await relations.Query<Tag>().AnyAsync(item => item.Id == replacementId, cancellationToken)) throw new BusinessException("InvalidReference");
        return await unitOfWork.ExecuteAsync(async () =>
        {
            var old = await GetAsync(parentId, targetId, cancellationToken);
            if (replacementId == targetId) return old;
            relations.Remove(old);
            var link = new ContactTag { OrganizationId = request.OrganizationId, ContactId = parentId, TagId = replacementId };
            relations.Add(link);
            return link;
        }, cancellationToken);
    }

    public async Task RemoveAsync(long parentId, long targetId, CancellationToken cancellationToken)
    {
        await CheckAsync(parentId, true, cancellationToken);
        await unitOfWork.ExecuteAsync(async () =>
        {
            relations.Remove(await GetAsync(parentId, targetId, cancellationToken));
            return true;
        }, cancellationToken);
    }
}
