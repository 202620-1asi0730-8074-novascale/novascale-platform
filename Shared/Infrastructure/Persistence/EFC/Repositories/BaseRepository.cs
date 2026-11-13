using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NovaScale.Platform.API.Shared.Domain.Model;
using NovaScale.Platform.API.Shared.Domain.Repositories;
using NovaScale.Platform.API.Shared.Infrastructure.Persistence.EFC.Configuration;

namespace NovaScale.Platform.API.Shared.Infrastructure.Persistence.EFC.Repositories;

public class BaseRepository<TEntity>(AppDbContext context) : IBaseRepository<TEntity> where TEntity : Entity
{
    protected readonly AppDbContext Context = context;

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default) =>
        await Context.Set<TEntity>().AddAsync(entity, cancellationToken);

    public Task<TEntity?> FindByIdAsync(long id, CancellationToken cancellationToken = default) =>
        Context.Set<TEntity>().SingleOrDefaultAsync(entity => entity.Id == id, cancellationToken);

    public async Task<IEnumerable<TEntity>> ListAsync(Expression<Func<TEntity, bool>> predicate, int page, int pageSize, CancellationToken cancellationToken = default) =>
        await Context.Set<TEntity>().Where(predicate).OrderByDescending(entity => entity.Id)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

    public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) =>
        Context.Set<TEntity>().CountAsync(predicate, cancellationToken);

    public void Remove(TEntity entity) => Context.Set<TEntity>().Remove(entity);
}
