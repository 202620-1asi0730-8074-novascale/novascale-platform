using NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates;
using NovaScale.Platform.API.IdentityAccess.Domain.Model.Commands;
using NovaScale.Platform.API.IdentityAccess.Domain.Model.Queries;
using NovaScale.Platform.API.Shared.Application.Patterns;

namespace NovaScale.Platform.API.IdentityAccess.Application.Services;

public interface IRoleQueryService
{
    Task<Role> Handle(GetRoleByIdQuery query, CancellationToken cancellationToken = default);
    Task<PageResult<Role>> Handle(GetAllRolesQuery query, CancellationToken cancellationToken = default);
}
