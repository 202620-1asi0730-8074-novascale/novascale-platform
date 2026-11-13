using Microsoft.AspNetCore.Identity;
using NovaScale.Platform.API.IdentityAccess.Application.Services;
using NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates;
using NovaScale.Platform.API.IdentityAccess.Domain.Model.Commands;
using NovaScale.Platform.API.IdentityAccess.Domain.Repositories;
using NovaScale.Platform.API.Shared.Application.Security;
using NovaScale.Platform.API.Shared.Application.Services;
using NovaScale.Platform.API.Shared.Domain.Exceptions;
using NovaScale.Platform.API.Shared.Domain.Repositories;

namespace NovaScale.Platform.API.IdentityAccess.Application.Internal.Services;

public class UserAccountService(IUserRepository repository, IUnitOfWork unitOfWork, IRequestContext request,
    IAccessPolicy access, IPasswordHasher<User> hasher, IEntityLifecycle<User> lifecycle) : IUserAccountService
{
    public async Task<User> CreateAsync(CreateUserCommand command, string password, CancellationToken cancellationToken = default)
    {
        await access.RequireAsync("manage_users", cancellationToken);
        if (password.Length is < 8 or > 128) throw new BusinessException("InvalidPassword");
        return await unitOfWork.ExecuteAsync(async () =>
        {
            await access.RequireAsync("manage_users", cancellationToken);
            var user = new User(command) { OrganizationId = request.OrganizationId };
            user.SetPasswordHash(hasher.HashPassword(user, password));
            await lifecycle.BeforeSaveAsync(user, true, cancellationToken);
            await repository.AddAsync(user, cancellationToken);
            return user;
        }, cancellationToken);
    }
    public async Task<User> UpdateAsync(long id, UpdateUserCommand command, string? password, CancellationToken cancellationToken = default)
    {
        await access.RequireAsync("manage_users", cancellationToken);
        if (password is not null && password.Length is < 8 or > 128) throw new BusinessException("InvalidPassword");
        return await unitOfWork.ExecuteAsync(async () =>
        {
            await access.RequireAsync("manage_users", cancellationToken);
            var user = await repository.FindByIdAsync(id, cancellationToken) ?? throw new BusinessException("NotFound", 404);
            user.Update(command);
            if (password is not null) user.SetPasswordHash(hasher.HashPassword(user, password));
            await lifecycle.BeforeSaveAsync(user, false, cancellationToken);
            return user;
        }, cancellationToken);
    }

}
