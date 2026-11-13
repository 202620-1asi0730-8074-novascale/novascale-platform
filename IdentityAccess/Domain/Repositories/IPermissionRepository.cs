using NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates;
using NovaScale.Platform.API.Shared.Domain.Repositories;

namespace NovaScale.Platform.API.IdentityAccess.Domain.Repositories;

public interface IPermissionRepository : IBaseRepository<Permission> { }
