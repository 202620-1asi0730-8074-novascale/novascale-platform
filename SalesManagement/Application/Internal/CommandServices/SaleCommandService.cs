using NovaScale.Platform.API.SalesManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.SalesManagement.Domain.Model.Commands;
using NovaScale.Platform.API.SalesManagement.Domain.Model.Queries;
using NovaScale.Platform.API.SalesManagement.Application.Services;
using NovaScale.Platform.API.SalesManagement.Domain.Repositories;
using NovaScale.Platform.API.Shared.Application.Security;
using NovaScale.Platform.API.Shared.Application.Services;
using NovaScale.Platform.API.Shared.Domain.Repositories;
using NovaScale.Platform.API.Shared.Application.Internal.CommandServices;

namespace NovaScale.Platform.API.SalesManagement.Application.Internal.CommandServices;

public class SaleCommandService(ISaleRepository repository, IUnitOfWork unitOfWork, IRequestContext request,
    IAccessPolicy access, IEntityLifecycle<Sale> lifecycle)
    : CrudCommandService<Sale>(repository, unitOfWork, request, access, lifecycle), ISaleCommandService
{
    public Task<Sale> Handle(CreateSaleCommand command, CancellationToken cancellationToken = default) =>
        CreateAsync(() => new Sale(command), "write_sales", cancellationToken);

    public Task<Sale> Handle(long id, UpdateSaleCommand command, CancellationToken cancellationToken = default) =>
        UpdateAsync(id, entity => entity.Update(command), "write_sales", cancellationToken);

    public Task RemoveAsync(long id, CancellationToken cancellationToken = default) =>
        DeleteAsync(id, "write_sales", cancellationToken);
}
