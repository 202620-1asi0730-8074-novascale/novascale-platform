using Microsoft.EntityFrameworkCore;
using NovaScale.Platform.API.OrganizationManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.Shared.Application.Security;
using NovaScale.Platform.API.Shared.Domain.Repositories;
using NovaScale.Platform.API.Shared.Infrastructure.Persistence.EFC.Configuration;

namespace NovaScale.Platform.API.Shared.Infrastructure.Persistence.EFC.Repositories;

public class UnitOfWork(AppDbContext context, IRequestContext request) : IUnitOfWork
{
    public async Task CompleteAsync(CancellationToken cancellationToken = default) =>
        await context.SaveChangesAsync(cancellationToken);

    public async Task<T> ExecuteAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken = default)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        if (request.OrganizationId > 0 && context.Database.ProviderName?.Contains("MySql", StringComparison.OrdinalIgnoreCase) == true)
            await context.Set<Organization>()
                .FromSqlInterpolated($"SELECT * FROM organizations WHERE id_organization = {request.OrganizationId} FOR UPDATE")
                .IgnoreQueryFilters().ToListAsync(cancellationToken);
        var result = await action();
        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return result;
    }
}
