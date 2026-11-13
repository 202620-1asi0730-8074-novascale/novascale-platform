using NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates;
using NovaScale.Platform.API.IdentityAccess.Domain.Model.Commands;
using NovaScale.Platform.API.IdentityAccess.Domain.Model.Queries;
using NovaScale.Platform.API.Shared.Application.Patterns;

namespace NovaScale.Platform.API.IdentityAccess.Application.Services;

public interface IPermissionQueryService
{
    Task<Permission> Handle(GetPermissionByIdQuery query, CancellationToken cancellationToken = default);
    Task<PageResult<Permission>> Handle(GetAllPermissionsQuery query, CancellationToken cancellationToken = default);
}
