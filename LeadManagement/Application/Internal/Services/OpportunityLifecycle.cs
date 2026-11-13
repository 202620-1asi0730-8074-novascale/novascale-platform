using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.LeadManagement.Domain.Model.Commands;
using NovaScale.Platform.API.SalesManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.SalesManagement.Domain.Model.Commands;
using NovaScale.Platform.API.Shared.Application.Security;
using NovaScale.Platform.API.Shared.Application.Services;
using NovaScale.Platform.API.Shared.Domain.Exceptions;
using NovaScale.Platform.API.Shared.Domain.Repositories;

namespace NovaScale.Platform.API.LeadManagement.Application.Internal.Services;

public class OpportunityLifecycle(IEntityRelations relations, EntityRules rules, IRequestContext request) : EntityLifecycle<Opportunity>
{
    public override Expression<Func<Opportunity, bool>> Scope(long userId, bool isOwner) => entity => isOwner || entity.UserId == userId;

    public override async Task BeforeSaveAsync(Opportunity entity, bool creating, CancellationToken cancellationToken)
    {
        await rules.RequireAssigneeAsync(entity.UserId, cancellationToken);
        var contact = await relations.Query<Contact>().SingleOrDefaultAsync(item => item.Id == entity.ContactId, cancellationToken)
            ?? throw new BusinessException("InvalidReference");
        await rules.RequireAssigneeAsync(contact.UserId, cancellationToken);
        if (entity.ExpectedCloseDate == DateOnly.MinValue) throw new BusinessException("InvalidData");
    }

    public override async Task AfterSaveAsync(Opportunity entity, bool creating, CancellationToken cancellationToken)
    {
        var sale = await relations.Query<Sale>().SingleOrDefaultAsync(item => item.OpportunityId == entity.Id, cancellationToken);
        if (entity.Status == "WON")
        {
            if (sale is null)
                relations.Add(new Sale(new CreateSaleCommand(entity.EstimatedAmount, DateOnly.FromDateTime(DateTime.UtcNow),
                    "CONFIRMED", "", entity.UserId, entity.Id)) { OrganizationId = entity.OrganizationId });
            else
                sale.Update(new UpdateSaleCommand(entity.EstimatedAmount, sale.SaleDate, "CONFIRMED", sale.Notes, sale.UserId, sale.OpportunityId));
        }
        else if (sale is not null)
            sale.Update(new UpdateSaleCommand(sale.Amount, sale.SaleDate, "CANCELLED", sale.Notes, sale.UserId, sale.OpportunityId));
        var contact = await relations.Query<Contact>().SingleAsync(item => item.Id == entity.ContactId, cancellationToken);
        if (entity.Status is "NEW" or "CONTACTED" or "PROPOSAL" or "WON") contact.ChangeStage(entity.Status);
        relations.Add(new Activity(new CreateActivityCommand("OPPORTUNITY_UPDATED", entity.Title, request.UserId, entity.ContactId))
            { OrganizationId = entity.OrganizationId });
    }

    public override async Task BeforeDeleteAsync(Opportunity entity, CancellationToken cancellationToken)
    {
        if (await relations.Query<Sale>().AnyAsync(item => item.OpportunityId == entity.Id, cancellationToken))
            throw new BusinessException("InUse", 409);
    }
}
