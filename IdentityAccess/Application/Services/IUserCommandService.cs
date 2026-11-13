using NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates;
using NovaScale.Platform.API.IdentityAccess.Domain.Model.Commands;
using NovaScale.Platform.API.IdentityAccess.Domain.Model.Queries;

namespace NovaScale.Platform.API.IdentityAccess.Application.Services;

public interface IUserCommandService
{
    Task<User> Handle(CreateUserCommand command, CancellationToken cancellationToken = default);
    Task<User> Handle(long id, UpdateUserCommand command, CancellationToken cancellationToken = default);
    Task RemoveAsync(long id, CancellationToken cancellationToken = default);
}
