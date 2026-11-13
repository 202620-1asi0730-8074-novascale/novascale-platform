using Microsoft.EntityFrameworkCore;
using NovaScale.Platform.API.SalesManagement.Application.Services;
using NovaScale.Platform.API.SalesManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates;
using NovaScale.Platform.API.Shared.Application.Security;
using NovaScale.Platform.API.Shared.Domain.Exceptions;
using NovaScale.Platform.API.Shared.Domain.Repositories;

namespace NovaScale.Platform.API.SalesManagement.Application.Internal.QueryServices;

public class DashboardQueryService(IEntityRelations relations, IRequestContext request, IAccessPolicy access) : IDashboardQueryService
{
    public async Task<DashboardSummary> GetAsync(DateOnly? from, DateOnly? to, long? userId, CancellationToken cancellationToken)
    {
        await access.RequireAsync("read_metrics", cancellationToken);
        var owner = await access.IsOwnerAsync(cancellationToken);
        if (!owner && userId.HasValue && userId != request.UserId) throw new BusinessException("Forbidden", 403);
        userId = owner ? userId : request.UserId;
        var (start, end) = Period(from, to);
        var startTime = start.ToDateTime(TimeOnly.MinValue);
        var endTime = end.AddDays(1).ToDateTime(TimeOnly.MinValue);
        var contacts = relations.Query<Contact>().Where(item => !userId.HasValue || item.UserId == userId);
        var opportunities = relations.Query<Opportunity>().Where(item => !userId.HasValue || item.UserId == userId);
        var sales = relations.Query<Sale>().Where(item => item.Status == "CONFIRMED" && item.SaleDate >= start && item.SaleDate <= end && (!userId.HasValue || item.UserId == userId));
        var cohort = contacts.Where(item => item.CreatedAt >= startTime && item.CreatedAt < endTime);
        var total = await cohort.CountAsync(cancellationToken);
        var converted = await cohort.CountAsync(contact => sales.Any(sale => opportunities.Any(opportunity => opportunity.Id == sale.OpportunityId && opportunity.ContactId == contact.Id)), cancellationToken);
        return new DashboardSummary(start, end, total, await cohort.CountAsync(item => item.ContactType == "LEAD", cancellationToken),
            await opportunities.CountAsync(item => item.Status != "WON" && item.Status != "LOST" && item.CreatedAt < endTime, cancellationToken),
            await sales.CountAsync(cancellationToken), await sales.SumAsync(item => (decimal?)item.Amount, cancellationToken) ?? 0,
            total == 0 ? 0 : decimal.Round(converted * 100m / total, 2),
            await relations.Query<User>().Where(item => item.Status == "ACTIVE" && (!userId.HasValue || item.Id == userId)).SumAsync(item => (decimal?)item.Quota, cancellationToken) ?? 0);
    }

    public async Task<IEnumerable<SalespersonSummary>> GetSalespeopleAsync(DateOnly? from, DateOnly? to, CancellationToken cancellationToken)
    {
        await access.RequireAsync("read_metrics", cancellationToken);
        if (!await access.IsOwnerAsync(cancellationToken)) throw new BusinessException("Forbidden", 403);
        var (start, end) = Period(from, to);
        var users = await relations.Query<User>().OrderBy(item => item.FullName).ToListAsync(cancellationToken);
        var sales = await relations.Query<Sale>().Where(item => item.Status == "CONFIRMED" && item.SaleDate >= start && item.SaleDate <= end)
            .GroupBy(item => item.UserId).Select(group => new { UserId = group.Key, Count = group.Count(), Amount = group.Sum(item => item.Amount) }).ToListAsync(cancellationToken);
        return users.Select(user => new SalespersonSummary(user.Id, user.FullName, sales.FirstOrDefault(item => item.UserId == user.Id)?.Count ?? 0,
            sales.FirstOrDefault(item => item.UserId == user.Id)?.Amount ?? 0));
    }

    private static (DateOnly Start, DateOnly End) Period(DateOnly? from, DateOnly? to)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var start = from ?? new DateOnly(today.Year, today.Month, 1);
        var end = to ?? today;
        if (start.Year < 1000 || end.Year < 1000 || end < start || end == DateOnly.MaxValue || end.DayNumber - start.DayNumber > 3660) throw new BusinessException("InvalidData");
        return (start, end);
    }
}
