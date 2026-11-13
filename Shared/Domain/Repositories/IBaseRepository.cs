using System.Linq.Expressions;
using NovaScale.Platform.API.Shared.Domain.Model;

namespace NovaScale.Platform.API.Shared.Domain.Repositories;

public interface IBaseRepository<TEntity> where TEntity : Entity
{
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<TEntity?> FindByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> ListAsync(Expression<Func<TEntity, bool>> predicate, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    void Remove(TEntity entity);
}
