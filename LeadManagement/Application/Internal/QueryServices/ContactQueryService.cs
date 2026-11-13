using NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates;
using NovaScale.Platform.API.SalesManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.Shared.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.LeadManagement.Domain.Model.Commands;
using NovaScale.Platform.API.LeadManagement.Domain.Model.Queries;
using NovaScale.Platform.API.LeadManagement.Application.Services;
using NovaScale.Platform.API.LeadManagement.Domain.Repositories;
using NovaScale.Platform.API.Shared.Application.Security;
using NovaScale.Platform.API.Shared.Application.Services;
using NovaScale.Platform.API.Shared.Domain.Repositories;
using NovaScale.Platform.API.Shared.Application.Internal.QueryServices;
using NovaScale.Platform.API.Shared.Application.Patterns;

namespace NovaScale.Platform.API.LeadManagement.Application.Internal.QueryServices;

public class ContactQueryService(IContactRepository repository, IRequestContext request, IAccessPolicy access,
    IEntityLifecycle<Contact> lifecycle, IEntityRelations relations) : CrudQueryService<Contact>(repository, request, access, lifecycle), IContactQueryService
{
    public Task<Contact> Handle(GetContactByIdQuery query, CancellationToken cancellationToken = default) =>
        FindAsync(query.Id, "read_contacts", cancellationToken);

    public Task<PageResult<Contact>> Handle(GetAllContactsQuery query, CancellationToken cancellationToken = default)
    {
        var search = query.Search?.Trim() ?? string.Empty;
        if (query.ContactType is not null && !new[] { "LEAD", "CUSTOMER" }.Contains(query.ContactType)) throw new BusinessException("InvalidState");
        if (query.Status is not null && !new[] { "NEW", "CONTACTED", "PROPOSAL", "WON" }.Contains(query.Status)) throw new BusinessException("InvalidState");
        var links = relations.Query<ContactTag>();
        return ListAsync(query.Page, query.PageSize, entity => (search == "" || entity.FullName.Contains(search) || entity.Email.Contains(search) || entity.Phone.Contains(search) || entity.Company.Contains(search) || entity.ContactType.Contains(search) || entity.LeadStatus.Contains(search) || entity.Source.Contains(search)) && (string.IsNullOrEmpty(query.ContactType) || entity.ContactType == query.ContactType) && (string.IsNullOrEmpty(query.Status) || entity.LeadStatus == query.Status) && (!query.UserId.HasValue || entity.UserId == query.UserId) && (!query.TagId.HasValue || links.Any(link => link.ContactId == entity.Id && link.TagId == query.TagId)), "read_contacts", cancellationToken);
    }
    public async Task<IReadOnlyList<ContactDetails>> GetDetailsAsync(IReadOnlyList<Contact> contacts, CancellationToken cancellationToken = default)
    {
        var ids = contacts.Select(contact => contact.Id).ToArray();
        if (ids.Length == 0) return [];
        var owner = await Access.IsOwnerAsync(cancellationToken);
        var opportunities = await relations.Query<Opportunity>().Where(item => ids.Contains(item.ContactId) && (owner || item.UserId == Request.UserId))
            .OrderByDescending(item => item.Id).ToListAsync(cancellationToken);
        var tags = await (from link in relations.Query<ContactTag>() join tag in relations.Query<Tag>() on link.TagId equals tag.Id
            where ids.Contains(link.ContactId) select new { link.ContactId, tag.Name }).ToListAsync(cancellationToken);
        var userIds = contacts.Select(contact => contact.UserId).ToArray();
        var users = await relations.Query<User>().Where(user => userIds.Contains(user.Id))
            .Select(user => new { user.Id, user.FullName }).ToListAsync(cancellationToken);
        var opportunityIds = opportunities.Select(item => item.Id).ToArray();
        var sales = await relations.Query<Sale>().Where(sale => opportunityIds.Contains(sale.OpportunityId) && sale.Status == "CONFIRMED" && (owner || sale.UserId == Request.UserId))
            .ToListAsync(cancellationToken);
        return contacts.Select(contact =>
        {
            var opportunity = opportunities.FirstOrDefault(item => item.ContactId == contact.Id);
            var sale = opportunity is null ? null : sales.FirstOrDefault(item => item.OpportunityId == opportunity.Id);
            return new ContactDetails(contact, opportunity?.EstimatedAmount ?? 0, opportunity?.Id,
                tags.Where(tag => tag.ContactId == contact.Id).Select(tag => tag.Name).ToArray(),
                users.FirstOrDefault(user => user.Id == contact.UserId)?.FullName ?? "",
                sale?.SaleDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc));
        }).ToArray();
    }

}
