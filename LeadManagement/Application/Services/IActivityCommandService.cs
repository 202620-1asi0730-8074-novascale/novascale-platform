using NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.LeadManagement.Domain.Model.Commands;
using NovaScale.Platform.API.LeadManagement.Domain.Model.Queries;

namespace NovaScale.Platform.API.LeadManagement.Application.Services;

public interface IActivityCommandService
{
    Task<Activity> Handle(CreateActivityCommand command, CancellationToken cancellationToken = default);
    Task<Activity> Handle(long id, UpdateActivityCommand command, CancellationToken cancellationToken = default);
    Task RemoveAsync(long id, CancellationToken cancellationToken = default);
}
