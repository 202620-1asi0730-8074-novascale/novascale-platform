using NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.Shared.Application.Services;
using NovaScale.Platform.API.Shared.Domain.Exceptions;

namespace NovaScale.Platform.API.LeadManagement.Application.Internal.Services;

public class TagLifecycle : EntityLifecycle<Tag>
{
    public override Task BeforeSaveAsync(Tag entity, bool creating, CancellationToken cancellationToken)
    {
        if (!System.Text.RegularExpressions.Regex.IsMatch(entity.Color, "^#[0-9a-fA-F]{6}$")) throw new BusinessException("InvalidData");
        return Task.CompletedTask;
    }
}
