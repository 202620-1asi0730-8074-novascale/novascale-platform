using NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates;
using NovaScale.Platform.API.IdentityAccess.Domain.Model.Commands;
using NovaScale.Platform.API.IdentityAccess.Domain.Model.Queries;
using NovaScale.Platform.API.Shared.Application.Patterns;

namespace NovaScale.Platform.API.IdentityAccess.Application.Services;

public interface IUserQueryService
{
    Task<User> Handle(GetUserByIdQuery query, CancellationToken cancellationToken = default);
    Task<PageResult<User>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken = default);
}
