using System.Linq.Expressions;
using NovaScale.Platform.API.Shared.Domain.Model;

namespace NovaScale.Platform.API.Shared.Application.Services;

public interface IEntityLifecycle<TEntity> where TEntity : Entity
{
    Expression<Func<TEntity, bool>> Scope(long userId, bool isOwner);
    Task BeforeSaveAsync(TEntity entity, bool creating, CancellationToken cancellationToken);
    Task AfterSaveAsync(TEntity entity, bool creating, CancellationToken cancellationToken);
    Task BeforeDeleteAsync(TEntity entity, CancellationToken cancellationToken);
}

public abstract class EntityLifecycle<TEntity> : IEntityLifecycle<TEntity> where TEntity : Entity
{
    public virtual Expression<Func<TEntity, bool>> Scope(long userId, bool isOwner) => entity => true;
    public virtual Task BeforeSaveAsync(TEntity entity, bool creating, CancellationToken cancellationToken) => Task.CompletedTask;
    public virtual Task AfterSaveAsync(TEntity entity, bool creating, CancellationToken cancellationToken) => Task.CompletedTask;
    public virtual Task BeforeDeleteAsync(TEntity entity, CancellationToken cancellationToken) => Task.CompletedTask;
}
