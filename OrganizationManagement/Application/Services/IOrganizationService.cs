using NovaScale.Platform.API.OrganizationManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.IdentityAccess.Domain.Model.Commands;

namespace NovaScale.Platform.API.OrganizationManagement.Application.Services;

public record ProvisionedOrganization(Organization Organization, string OwnerToken);

public interface IOrganizationService
{
    Task<IEnumerable<Organization>> ListAsync(CancellationToken cancellationToken);
    Task<Organization> GetAsync(long id, CancellationToken cancellationToken);
    Task<ProvisionedOrganization> CreateAsync(SignUpCommand command, CancellationToken cancellationToken);
    Task<Organization> UpdateAsync(long id, string name, CancellationToken cancellationToken);
    Task DeleteAsync(long id, CancellationToken cancellationToken);
}
