using NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.LeadManagement.Domain.Model.Commands;
using NovaScale.Platform.API.LeadManagement.Domain.Model.Queries;
using NovaScale.Platform.API.LeadManagement.Application.Services;
using NovaScale.Platform.API.LeadManagement.Domain.Repositories;
using NovaScale.Platform.API.Shared.Application.Security;
using NovaScale.Platform.API.Shared.Application.Services;
using NovaScale.Platform.API.Shared.Domain.Repositories;
using NovaScale.Platform.API.Shared.Application.Internal.CommandServices;

namespace NovaScale.Platform.API.LeadManagement.Application.Internal.CommandServices;

public class ActivityCommandService(IActivityRepository repository, IUnitOfWork unitOfWork, IRequestContext request,
    IAccessPolicy access, IEntityLifecycle<Activity> lifecycle)
    : CrudCommandService<Activity>(repository, unitOfWork, request, access, lifecycle), IActivityCommandService
{
    public Task<Activity> Handle(CreateActivityCommand command, CancellationToken cancellationToken = default) =>
        CreateAsync(() => new Activity(command), "write_contacts", cancellationToken);

    public Task<Activity> Handle(long id, UpdateActivityCommand command, CancellationToken cancellationToken = default) =>
        UpdateAsync(id, entity => entity.Update(command), "write_contacts", cancellationToken);

    public Task RemoveAsync(long id, CancellationToken cancellationToken = default) =>
        DeleteAsync(id, "manage_users", cancellationToken);
}
