using Microsoft.EntityFrameworkCore;
using NovaScale.Platform.API.OrganizationManagement.Application.Services;
using NovaScale.Platform.API.OrganizationManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.IdentityAccess.Application.Services;
using NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates;
using NovaScale.Platform.API.IdentityAccess.Domain.Model.Commands;
using NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.SalesManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.Shared.Application.Security;
using NovaScale.Platform.API.Shared.Domain.Exceptions;
using NovaScale.Platform.API.Shared.Domain.Repositories;

namespace NovaScale.Platform.API.OrganizationManagement.Application.Internal.Services;

public class OrganizationService(IEntityRelations relations, IUnitOfWork unitOfWork, IAccessPolicy access,
    IAuthenticationService authentication) : IOrganizationService
{
    public async Task<IEnumerable<Organization>> ListAsync(CancellationToken cancellationToken)
    {
        await access.RequireAsync("read_users", cancellationToken);
        return await relations.Query<Organization>().ToListAsync(cancellationToken);
    }

    public async Task<Organization> GetAsync(long id, CancellationToken cancellationToken)
    {
        await access.RequireAsync("read_users", cancellationToken);
        return await relations.Query<Organization>().SingleOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new BusinessException("NotFound", 404);
    }

    public async Task<ProvisionedOrganization> CreateAsync(SignUpCommand command, CancellationToken cancellationToken)
    {
        await access.RequireAsync("manage_users", cancellationToken);
        var result = await authentication.SignUpAsync(command, cancellationToken);
        var organization = await relations.Query<Organization>().IgnoreQueryFilters()
            .SingleAsync(item => item.Id == result.User.OrganizationId, cancellationToken);
        return new ProvisionedOrganization(organization, result.Token);
    }

    public async Task<Organization> UpdateAsync(long id, string name, CancellationToken cancellationToken)
    {
        await access.RequireAsync("manage_users", cancellationToken);
        return await unitOfWork.ExecuteAsync(async () =>
        {
            var organization = await GetAsync(id, cancellationToken);
            organization.Rename(name);
            return organization;
        }, cancellationToken);
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken)
    {
        await access.RequireAsync("manage_users", cancellationToken);
        await unitOfWork.ExecuteAsync(async () =>
        {
            var organization = await GetAsync(id, cancellationToken);
            if (await relations.Query<Contact>().AnyAsync(cancellationToken) || await relations.Query<Opportunity>().AnyAsync(cancellationToken)
                || await relations.Query<Conversation>().AnyAsync(cancellationToken) || await relations.Query<Sale>().AnyAsync(cancellationToken)
                || await relations.Query<Activity>().AnyAsync(cancellationToken)) throw new BusinessException("InUse", 409);
            foreach (var link in await relations.Query<RolePermission>().ToListAsync(cancellationToken)) relations.Remove(link);
            foreach (var user in await relations.Query<User>().ToListAsync(cancellationToken)) relations.Remove(user);
            foreach (var role in await relations.Query<Role>().ToListAsync(cancellationToken)) relations.Remove(role);
            foreach (var permission in await relations.Query<Permission>().ToListAsync(cancellationToken)) relations.Remove(permission);
            foreach (var tag in await relations.Query<Tag>().ToListAsync(cancellationToken)) relations.Remove(tag);
            relations.Remove(organization);
            return true;
        }, cancellationToken);
    }
}
