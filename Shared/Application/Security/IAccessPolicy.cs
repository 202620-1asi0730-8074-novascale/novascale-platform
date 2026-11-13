namespace NovaScale.Platform.API.Shared.Application.Security;

public interface IAccessPolicy
{
    Task RequireAsync(string permission, CancellationToken cancellationToken = default);
    Task<bool> IsOwnerAsync(CancellationToken cancellationToken = default);
}
