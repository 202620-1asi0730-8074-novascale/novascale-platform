using System.Security.Claims;
using NovaScale.Platform.API.Shared.Application.Security;

namespace NovaScale.Platform.API.Shared.Infrastructure.Security;

public class RequestContext(IHttpContextAccessor accessor) : IRequestContext
{
    public long UserId => long.TryParse(accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
    public long OrganizationId => long.TryParse(accessor.HttpContext?.User.FindFirstValue("organization_id"), out var id) ? id : 0;
}
