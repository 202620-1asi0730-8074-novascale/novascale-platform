using NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.LeadManagement.Domain.Model.Commands;
using NovaScale.Platform.API.LeadManagement.Domain.Model.Queries;
using NovaScale.Platform.API.Shared.Application.Patterns;

namespace NovaScale.Platform.API.LeadManagement.Application.Services;

public interface IActivityQueryService
{
    Task<Activity> Handle(GetActivityByIdQuery query, CancellationToken cancellationToken = default);
    Task<PageResult<Activity>> Handle(GetAllActivitiesQuery query, CancellationToken cancellationToken = default);
}
