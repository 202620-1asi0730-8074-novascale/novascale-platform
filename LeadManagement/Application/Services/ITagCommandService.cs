using NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.LeadManagement.Domain.Model.Commands;
using NovaScale.Platform.API.LeadManagement.Domain.Model.Queries;

namespace NovaScale.Platform.API.LeadManagement.Application.Services;

public interface ITagCommandService
{
    Task<Tag> Handle(CreateTagCommand command, CancellationToken cancellationToken = default);
    Task<Tag> Handle(long id, UpdateTagCommand command, CancellationToken cancellationToken = default);
    Task RemoveAsync(long id, CancellationToken cancellationToken = default);
}
