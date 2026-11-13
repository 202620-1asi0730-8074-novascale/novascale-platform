using System.Linq.Expressions;
using NovaScale.Platform.API.Shared.Application.Patterns;
using NovaScale.Platform.API.Shared.Application.Security;
using NovaScale.Platform.API.Shared.Application.Services;
using NovaScale.Platform.API.Shared.Domain.Exceptions;
using NovaScale.Platform.API.Shared.Domain.Model;
using NovaScale.Platform.API.Shared.Domain.Repositories;

namespace NovaScale.Platform.API.Shared.Application.Internal.QueryServices;

public abstract class CrudQueryService<TEntity>(IBaseRepository<TEntity> repository, IRequestContext request,
    IAccessPolicy access, IEntityLifecycle<TEntity> lifecycle) where TEntity : Entity
{
    protected IRequestContext Request => request;
    protected IAccessPolicy Access => access;

    protected async Task<TEntity> FindAsync(long id, string permission, CancellationToken cancellationToken)
    {
        await access.RequireAsync(permission, cancellationToken);
        var entity = await repository.FindByIdAsync(id, cancellationToken) ?? throw new BusinessException("NotFound", 404);
        if (!lifecycle.Scope(request.UserId, await access.IsOwnerAsync(cancellationToken)).Compile()(entity))
            throw new BusinessException("Forbidden", 403);
        return entity;
    }

    protected async Task<PageResult<TEntity>> ListAsync(int page, int pageSize, Expression<Func<TEntity, bool>> search,
        string permission, CancellationToken cancellationToken)
    {
        await access.RequireAsync(permission, cancellationToken);
        if (page < 1 || pageSize is < 1 or > 100 || page > 100000) throw new BusinessException("InvalidPagination");
        var scope = lifecycle.Scope(request.UserId, await access.IsOwnerAsync(cancellationToken));
        var parameter = scope.Parameters[0];
        var body = new ParameterReplacement(search.Parameters[0], parameter).Visit(search.Body)!;
        var predicate = Expression.Lambda<Func<TEntity, bool>>(Expression.AndAlso(scope.Body, body), parameter);
        return new PageResult<TEntity>(await repository.ListAsync(predicate, page, pageSize, cancellationToken),
            await repository.CountAsync(predicate, cancellationToken), page, pageSize);
    }

    private sealed class ParameterReplacement(ParameterExpression source, ParameterExpression target) : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node) => node == source ? target : node;
    }
}
