using NovaScale.Platform.API.Shared.Application.Security;
using NovaScale.Platform.API.Shared.Application.Services;
using NovaScale.Platform.API.Shared.Domain.Exceptions;
using NovaScale.Platform.API.Shared.Domain.Model;
using NovaScale.Platform.API.Shared.Domain.Repositories;

namespace NovaScale.Platform.API.Shared.Application.Internal.CommandServices;

public abstract class CrudCommandService<TEntity>(IBaseRepository<TEntity> repository, IUnitOfWork unitOfWork,
    IRequestContext request, IAccessPolicy access, IEntityLifecycle<TEntity> lifecycle) where TEntity : Entity
{
    protected async Task<TEntity> CreateAsync(Func<TEntity> create, string permission, CancellationToken cancellationToken)
    {
        await access.RequireAsync(permission, cancellationToken);
        return await unitOfWork.ExecuteAsync(async () =>
        {
            await access.RequireAsync(permission, cancellationToken);
            var entity = create();
            entity.OrganizationId = request.OrganizationId;
            await lifecycle.BeforeSaveAsync(entity, true, cancellationToken);
            await repository.AddAsync(entity, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            await lifecycle.AfterSaveAsync(entity, true, cancellationToken);
            return entity;
        }, cancellationToken);
    }

    protected async Task<TEntity> UpdateAsync(long id, Action<TEntity> update, string permission, CancellationToken cancellationToken)
    {
        await access.RequireAsync(permission, cancellationToken);
        return await unitOfWork.ExecuteAsync(async () =>
        {
            await access.RequireAsync(permission, cancellationToken);
            var entity = await AccessibleAsync(id, cancellationToken);
            update(entity);
            await lifecycle.BeforeSaveAsync(entity, false, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            await lifecycle.AfterSaveAsync(entity, false, cancellationToken);
            return entity;
        }, cancellationToken);
    }

    public async Task DeleteAsync(long id, string permission, CancellationToken cancellationToken)
    {
        await access.RequireAsync(permission, cancellationToken);
        await unitOfWork.ExecuteAsync(async () =>
        {
            await access.RequireAsync(permission, cancellationToken);
            var entity = await AccessibleAsync(id, cancellationToken);
            await lifecycle.BeforeDeleteAsync(entity, cancellationToken);
            repository.Remove(entity);
            return true;
        }, cancellationToken);
    }

    private async Task<TEntity> AccessibleAsync(long id, CancellationToken cancellationToken)
    {
        var entity = await repository.FindByIdAsync(id, cancellationToken) ?? throw new BusinessException("NotFound", 404);
        if (!lifecycle.Scope(request.UserId, await access.IsOwnerAsync(cancellationToken)).Compile()(entity))
            throw new BusinessException("Forbidden", 403);
        return entity;
    }
}
