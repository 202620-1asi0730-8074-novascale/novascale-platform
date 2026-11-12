namespace NovaScale.Platform.API.Shared.Domain.Repositories;

public interface IEntityRelations
{
    IQueryable<TEntity> Query<TEntity>() where TEntity : class;
    void Add<TEntity>(TEntity entity) where TEntity : class;
    void Remove<TEntity>(TEntity entity) where TEntity : class;
}
