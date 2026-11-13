using NovaScale.Platform.API.SalesManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.SalesManagement.Domain.Model.Commands;
using NovaScale.Platform.API.SalesManagement.Domain.Model.Queries;
using NovaScale.Platform.API.SalesManagement.Application.Services;
using NovaScale.Platform.API.SalesManagement.Domain.Repositories;
using NovaScale.Platform.API.Shared.Application.Security;
using NovaScale.Platform.API.Shared.Application.Services;
using NovaScale.Platform.API.Shared.Domain.Repositories;
using NovaScale.Platform.API.Shared.Application.Internal.QueryServices;
using NovaScale.Platform.API.Shared.Application.Patterns;

namespace NovaScale.Platform.API.SalesManagement.Application.Internal.QueryServices;

public class SaleQueryService(ISaleRepository repository, IRequestContext request, IAccessPolicy access,
    IEntityLifecycle<Sale> lifecycle) : CrudQueryService<Sale>(repository, request, access, lifecycle), ISaleQueryService
{
    public Task<Sale> Handle(GetSaleByIdQuery query, CancellationToken cancellationToken = default) =>
        FindAsync(query.Id, "read_sales", cancellationToken);

    public Task<PageResult<Sale>> Handle(GetAllSalesQuery query, CancellationToken cancellationToken = default)
    {
        var search = query.Search?.Trim() ?? string.Empty;
        return ListAsync(query.Page, query.PageSize, entity => search == "" || entity.Status.Contains(search) || entity.Notes.Contains(search), "read_sales", cancellationToken);
    }
}
