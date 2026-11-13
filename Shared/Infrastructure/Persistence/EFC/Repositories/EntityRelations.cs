using NovaScale.Platform.API.Shared.Domain.Repositories;
using NovaScale.Platform.API.Shared.Infrastructure.Persistence.EFC.Configuration;

namespace NovaScale.Platform.API.Shared.Infrastructure.Persistence.EFC.Repositories;

public class EntityRelations(AppDbContext context) : IEntityRelations
{
    public IQueryable<TEntity> Query<TEntity>() where TEntity : class => context.Set<TEntity>();
    public void Add<TEntity>(TEntity entity) where TEntity : class => context.Set<TEntity>().Add(entity);
    public void Remove<TEntity>(TEntity entity) where TEntity : class => context.Set<TEntity>().Remove(entity);
}
