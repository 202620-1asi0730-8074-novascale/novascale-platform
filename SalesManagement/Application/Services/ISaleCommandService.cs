using NovaScale.Platform.API.SalesManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.SalesManagement.Domain.Model.Commands;
using NovaScale.Platform.API.SalesManagement.Domain.Model.Queries;

namespace NovaScale.Platform.API.SalesManagement.Application.Services;

public interface ISaleCommandService
{
    Task<Sale> Handle(CreateSaleCommand command, CancellationToken cancellationToken = default);
    Task<Sale> Handle(long id, UpdateSaleCommand command, CancellationToken cancellationToken = default);
    Task RemoveAsync(long id, CancellationToken cancellationToken = default);
}
