using NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates;
using NovaScale.Platform.API.IdentityAccess.Domain.Model.Commands;

namespace NovaScale.Platform.API.IdentityAccess.Application.Services;

public record AuthenticationResult(string Token, User User);

public interface IAuthenticationService
{
    Task<AuthenticationResult> SignInAsync(SignInCommand command, CancellationToken cancellationToken = default);
    Task<AuthenticationResult> SignUpAsync(SignUpCommand command, CancellationToken cancellationToken = default);
    Task<User> GetProfileAsync(CancellationToken cancellationToken = default);
    Task<User> UpdateProfileAsync(UpdateProfileCommand command, CancellationToken cancellationToken = default);
    Task ChangePasswordAsync(ChangePasswordCommand command, CancellationToken cancellationToken = default);
}
