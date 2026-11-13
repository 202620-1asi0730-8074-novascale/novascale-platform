using NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates;
using NovaScale.Platform.API.IdentityAccess.Domain.Repositories;
using NovaScale.Platform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using NovaScale.Platform.API.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace NovaScale.Platform.API.IdentityAccess.Infrastructure.Persistence.EFC.Repositories;

public class RoleRepository(AppDbContext context) : BaseRepository<Role>(context), IRoleRepository { }
