using NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.LeadManagement.Domain.Repositories;
using NovaScale.Platform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using NovaScale.Platform.API.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace NovaScale.Platform.API.LeadManagement.Infrastructure.Persistence.EFC.Repositories;

public class ActivityRepository(AppDbContext context) : BaseRepository<Activity>(context), IActivityRepository { }
