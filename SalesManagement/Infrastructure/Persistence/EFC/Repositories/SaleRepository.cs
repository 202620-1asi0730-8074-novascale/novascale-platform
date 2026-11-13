using NovaScale.Platform.API.SalesManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.SalesManagement.Domain.Repositories;
using NovaScale.Platform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using NovaScale.Platform.API.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace NovaScale.Platform.API.SalesManagement.Infrastructure.Persistence.EFC.Repositories;

public class SaleRepository(AppDbContext context) : BaseRepository<Sale>(context), ISaleRepository { }
