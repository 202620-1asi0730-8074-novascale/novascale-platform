using NovaScale.Platform.API.SalesManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.SalesManagement.Domain.Model.Commands;
using NovaScale.Platform.API.SalesManagement.Domain.Model.Queries;
using NovaScale.Platform.API.Shared.Application.Patterns;

namespace NovaScale.Platform.API.SalesManagement.Application.Services;

public interface ISaleQueryService
{
    Task<Sale> Handle(GetSaleByIdQuery query, CancellationToken cancellationToken = default);
    Task<PageResult<Sale>> Handle(GetAllSalesQuery query, CancellationToken cancellationToken = default);
}
