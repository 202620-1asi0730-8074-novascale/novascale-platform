using NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates;

namespace NovaScale.Platform.API.IdentityAccess.Domain.Services;

public interface ITokenProvider
{
    string Generate(User user);
}
