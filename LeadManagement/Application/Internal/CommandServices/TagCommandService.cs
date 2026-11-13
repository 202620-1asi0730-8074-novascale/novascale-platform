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

public class TagCommandService(ITagRepository repository, IUnitOfWork unitOfWork, IRequestContext request,
    IAccessPolicy access, IEntityLifecycle<Tag> lifecycle)
    : CrudCommandService<Tag>(repository, unitOfWork, request, access, lifecycle), ITagCommandService
{
    public Task<Tag> Handle(CreateTagCommand command, CancellationToken cancellationToken = default) =>
        CreateAsync(() => new Tag(command), "manage_tags", cancellationToken);

    public Task<Tag> Handle(long id, UpdateTagCommand command, CancellationToken cancellationToken = default) =>
        UpdateAsync(id, entity => entity.Update(command), "manage_tags", cancellationToken);

    public Task RemoveAsync(long id, CancellationToken cancellationToken = default) =>
        DeleteAsync(id, "manage_tags", cancellationToken);
}
