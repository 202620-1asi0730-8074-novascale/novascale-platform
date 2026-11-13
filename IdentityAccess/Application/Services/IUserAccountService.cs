using NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates;
using NovaScale.Platform.API.IdentityAccess.Domain.Model.Commands;

namespace NovaScale.Platform.API.IdentityAccess.Application.Services;

public interface IUserAccountService
{
    Task<User> CreateAsync(CreateUserCommand command, string password, CancellationToken cancellationToken = default);
    Task<User> UpdateAsync(long id, UpdateUserCommand command, string? password, CancellationToken cancellationToken = default);
}
