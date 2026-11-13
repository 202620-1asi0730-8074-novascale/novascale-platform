using Microsoft.EntityFrameworkCore;
using NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.LeadManagement.Domain.Model.Commands;
using NovaScale.Platform.API.LeadManagement.Application.Services;
using NovaScale.Platform.API.LeadManagement.Domain.Repositories;
using NovaScale.Platform.API.SalesManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.Shared.Application.Security;
using NovaScale.Platform.API.Shared.Application.Services;
using NovaScale.Platform.API.Shared.Domain.Model;
using NovaScale.Platform.API.Shared.Domain.Exceptions;
using NovaScale.Platform.API.Shared.Domain.Repositories;

namespace NovaScale.Platform.API.LeadManagement.Application.Internal.CommandServices;

public class ContactCommandService(IContactRepository repository, IUnitOfWork unitOfWork, IRequestContext request,
    IAccessPolicy access, IEntityLifecycle<Contact> lifecycle, IEntityRelations relations,
    IEntityLifecycle<Opportunity> opportunityLifecycle) : IContactCommandService
{
    public async Task<Contact> Handle(CreateContactCommand command, CancellationToken cancellationToken = default)
    {
        await access.RequireAsync("write_contacts", cancellationToken);
        return await unitOfWork.ExecuteAsync(async () =>
        {
            await access.RequireAsync("write_contacts", cancellationToken);
            var contact = new Contact(command) { OrganizationId = request.OrganizationId };
            await lifecycle.BeforeSaveAsync(contact, true, cancellationToken);
            await repository.AddAsync(contact, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            await lifecycle.AfterSaveAsync(contact, true, cancellationToken);
            await SaveDetailsAsync(contact, command.EstimatedAmount, command.Tags, cancellationToken);
            return contact;
        }, cancellationToken);
    }

    public async Task<Contact> Handle(long id, UpdateContactCommand command, CancellationToken cancellationToken = default)
    {
        await access.RequireAsync("write_contacts", cancellationToken);
        return await unitOfWork.ExecuteAsync(async () =>
        {
            await access.RequireAsync("write_contacts", cancellationToken);
            var contact = await AccessibleAsync(id, cancellationToken);
            contact.Update(command);
            await lifecycle.BeforeSaveAsync(contact, false, cancellationToken);
            await lifecycle.AfterSaveAsync(contact, false, cancellationToken);
            await SaveDetailsAsync(contact, command.EstimatedAmount, command.Tags, cancellationToken);
            return contact;
        }, cancellationToken);
    }

    private async Task SaveDetailsAsync(Contact contact, decimal? amount, IReadOnlyList<string>? names, CancellationToken cancellationToken)
    {
        if (names is not null)
        {
            if (names.Count > 30) throw new BusinessException("InvalidData");
            var wanted = names.Select(name => Guard.Text(name, 50)).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
            var links = await relations.Query<ContactTag>().Where(link => link.ContactId == contact.Id).ToListAsync(cancellationToken);
            var tags = await relations.Query<Tag>().ToListAsync(cancellationToken);
            var selected = new HashSet<long>();
            foreach (var name in wanted)
            {
                var tag = tags.FirstOrDefault(tag => string.Equals(tag.Name, name, StringComparison.OrdinalIgnoreCase));
                if (tag is null)
                {
                    await access.RequireAsync("manage_tags", cancellationToken);
                    tag = new Tag(new CreateTagCommand(name, "#6366f1")) { OrganizationId = request.OrganizationId };
                    relations.Add(tag);
                    await unitOfWork.CompleteAsync(cancellationToken);
                    tags.Add(tag);
                }
                selected.Add(tag.Id);
                if (!links.Any(link => link.TagId == tag.Id))
                    relations.Add(new ContactTag { OrganizationId = request.OrganizationId, ContactId = contact.Id, TagId = tag.Id });
            }
            foreach (var link in links.Where(link => !selected.Contains(link.TagId))) relations.Remove(link);
        }
        if (!amount.HasValue) return;
        Guard.Amount(amount.Value);
        var owner = await access.IsOwnerAsync(cancellationToken);
        var opportunity = await relations.Query<Opportunity>().Where(item => item.ContactId == contact.Id && (owner || item.UserId == request.UserId))
            .OrderByDescending(item => item.Id).FirstOrDefaultAsync(cancellationToken);
        var creating = opportunity is null;
        if (opportunity is null)
        {
            opportunity = new Opportunity(new CreateOpportunityCommand(contact.FullName, amount.Value, contact.LeadStatus, null, contact.UserId, contact.Id))
                { OrganizationId = request.OrganizationId };
            relations.Add(opportunity);
        }
        else opportunity.Update(new UpdateOpportunityCommand(contact.FullName, amount.Value, contact.LeadStatus,
            opportunity.ExpectedCloseDate, opportunity.UserId, contact.Id));
        await opportunityLifecycle.BeforeSaveAsync(opportunity, creating, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        await opportunityLifecycle.AfterSaveAsync(opportunity, creating, cancellationToken);
    }

    public async Task RemoveAsync(long id, CancellationToken cancellationToken = default)
    {
        await access.RequireAsync("delete_contacts", cancellationToken);
        await unitOfWork.ExecuteAsync(async () =>
        {
            await access.RequireAsync("delete_contacts", cancellationToken);
            var contact = await AccessibleAsync(id, cancellationToken);
            var owner = await access.IsOwnerAsync(cancellationToken);
            var opportunities = await relations.Query<Opportunity>().Where(item => item.ContactId == id).ToListAsync(cancellationToken);
            if (!owner && opportunities.Any(item => item.UserId != request.UserId)) throw new BusinessException("Forbidden", 403);
            var ids = opportunities.Select(item => item.Id).ToArray();
            var sales = await relations.Query<Sale>().Where(item => ids.Contains(item.OpportunityId)).ToListAsync(cancellationToken);
            if (!owner && sales.Any(item => item.UserId != request.UserId)) throw new BusinessException("Forbidden", 403);
            foreach (var sale in sales) relations.Remove(sale);
            foreach (var opportunity in opportunities) relations.Remove(opportunity);
            await unitOfWork.CompleteAsync(cancellationToken);
            await lifecycle.BeforeDeleteAsync(contact, cancellationToken);
            repository.Remove(contact);
            return true;
        }, cancellationToken);
    }

    private async Task<Contact> AccessibleAsync(long id, CancellationToken cancellationToken)
    {
        var contact = await repository.FindByIdAsync(id, cancellationToken) ?? throw new BusinessException("NotFound", 404);
        if (!lifecycle.Scope(request.UserId, await access.IsOwnerAsync(cancellationToken)).Compile()(contact)) throw new BusinessException("Forbidden", 403);
        return contact;
    }
}
