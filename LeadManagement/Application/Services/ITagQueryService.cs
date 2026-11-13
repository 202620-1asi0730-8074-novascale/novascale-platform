using NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.LeadManagement.Domain.Model.Commands;
using NovaScale.Platform.API.LeadManagement.Domain.Model.Queries;
using NovaScale.Platform.API.Shared.Application.Patterns;

namespace NovaScale.Platform.API.LeadManagement.Application.Services;

public interface ITagQueryService
{
    Task<Tag> Handle(GetTagByIdQuery query, CancellationToken cancellationToken = default);
    Task<PageResult<Tag>> Handle(GetAllTagsQuery query, CancellationToken cancellationToken = default);
}
