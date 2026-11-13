using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NovaScale.Platform.API.SalesManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.LeadManagement.Domain.Model.Commands;
using NovaScale.Platform.API.Shared.Application.Services;
using NovaScale.Platform.API.Shared.Domain.Exceptions;
using NovaScale.Platform.API.Shared.Domain.Repositories;

namespace NovaScale.Platform.API.SalesManagement.Application.Internal.Services;

public class SaleLifecycle(IEntityRelations relations, EntityRules rules) : EntityLifecycle<Sale>
{
    public override Expression<Func<Sale, bool>> Scope(long userId, bool isOwner) => entity => isOwner || entity.UserId == userId;

    public override async Task BeforeSaveAsync(Sale entity, bool creating, CancellationToken cancellationToken)
    {
        await rules.RequireAssigneeAsync(entity.UserId, cancellationToken);
        var opportunity = await relations.Query<Opportunity>().SingleOrDefaultAsync(item => item.Id == entity.OpportunityId, cancellationToken)
            ?? throw new BusinessException("InvalidReference");
        await rules.RequireAssigneeAsync(opportunity.UserId, cancellationToken);
        if (creating && opportunity.UserId != entity.UserId) throw new BusinessException("InvalidReference");
        if (!creating)
        {
            var original = await relations.Query<Sale>().AsNoTracking().SingleAsync(item => item.Id == entity.Id, cancellationToken);
            if (original.UserId != entity.UserId || original.OpportunityId != entity.OpportunityId)
                throw new BusinessException("InvalidReference");
        }
        if (entity.SaleDate == DateOnly.MinValue || entity.SaleDate > DateOnly.FromDateTime(DateTime.UtcNow))
            throw new BusinessException("InvalidData");
        var status = entity.Status == "CONFIRMED" ? "WON" : opportunity.Status == "WON" ? "CONTACTED" : opportunity.Status;
        opportunity.Update(new UpdateOpportunityCommand(opportunity.Title, entity.Amount, status, opportunity.ExpectedCloseDate,
            opportunity.UserId, opportunity.ContactId));
        var contact = await relations.Query<Contact>().SingleAsync(item => item.Id == opportunity.ContactId, cancellationToken);
        if (status is "NEW" or "CONTACTED" or "PROPOSAL" or "WON") contact.ChangeStage(status);
    }

    public override Task BeforeDeleteAsync(Sale entity, CancellationToken cancellationToken)
    {
        if (entity.Status != "CANCELLED") throw new BusinessException("CancelSaleFirst", 409);
        return Task.CompletedTask;
    }
}
