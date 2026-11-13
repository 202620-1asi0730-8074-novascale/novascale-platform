using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NovaScale.Platform.API.IdentityAccess.Application.Services;
using NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates;
using NovaScale.Platform.API.IdentityAccess.Domain.Model.Commands;
using NovaScale.Platform.API.IdentityAccess.Domain.Services;
using NovaScale.Platform.API.OrganizationManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.Shared.Application.Security;
using NovaScale.Platform.API.Shared.Domain.Exceptions;
using NovaScale.Platform.API.Shared.Domain.Model;
using NovaScale.Platform.API.Shared.Domain.Repositories;

namespace NovaScale.Platform.API.IdentityAccess.Application.Internal.Services;

public class AuthenticationService(IEntityRelations relations, IUnitOfWork unitOfWork, IPasswordHasher<User> hasher,
    ITokenProvider tokenProvider, IRequestContext request, IAccessPolicy access) : IAuthenticationService
{
    public async Task<AuthenticationResult> SignInAsync(SignInCommand command, CancellationToken cancellationToken = default)
    {
        var email = Guard.Email(command.Email);
        var user = await relations.Query<User>().IgnoreQueryFilters().SingleOrDefaultAsync(user => user.Email == email, cancellationToken);
        var passwordResult = hasher.VerifyHashedPassword(user!, user?.PasswordHash ?? DummyHash, command.Password);
        if (user is null || user.Status != "ACTIVE" || passwordResult == PasswordVerificationResult.Failed)
            throw new BusinessException("InvalidCredentials", 401);
        if (passwordResult == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.SetPasswordHash(hasher.HashPassword(user, command.Password));
            await unitOfWork.CompleteAsync(cancellationToken);
        }
        return new AuthenticationResult(tokenProvider.Generate(user), user);
    }

    public async Task<AuthenticationResult> SignUpAsync(SignUpCommand command, CancellationToken cancellationToken = default)
    {
        if (command.Password.Length is < 8 or > 128) throw new BusinessException("InvalidPassword");
        var email = Guard.Email(command.Email);
        if (await relations.Query<User>().IgnoreQueryFilters().AnyAsync(user => user.Email == email, cancellationToken))
            throw new BusinessException("Duplicate", 409);
        return await unitOfWork.ExecuteAsync(async () =>
        {
            var organization = new Organization(command.Company);
            relations.Add(organization);
            await unitOfWork.CompleteAsync(cancellationToken);
            var owner = new Role(new CreateRoleCommand("Owner", "Business owner")) { OrganizationId = organization.Id };
            owner.SetSystemCode("OWNER");
            var seller = new Role(new CreateRoleCommand("Salesperson", "Sales team member")) { OrganizationId = organization.Id };
            seller.SetSystemCode("SELLER");
            relations.Add(owner);
            relations.Add(seller);
            await unitOfWork.CompleteAsync(cancellationToken);
            string[] codes = ["read_users", "manage_users", "manage_roles", "manage_permissions", "read_contacts", "write_contacts",
                "delete_contacts", "manage_tags", "read_conversations", "write_conversations", "read_sales", "write_sales", "read_metrics", "export_data"];
            foreach (var code in codes)
            {
                var permission = new Permission(new CreatePermissionCommand(code)) { OrganizationId = organization.Id };
                relations.Add(permission);
                await unitOfWork.CompleteAsync(cancellationToken);
                relations.Add(new RolePermission { OrganizationId = organization.Id, RoleId = owner.Id, PermissionId = permission.Id });
                if (!code.StartsWith("manage_") || code == "manage_tags")
                    relations.Add(new RolePermission { OrganizationId = organization.Id, RoleId = seller.Id, PermissionId = permission.Id });
            }
            var user = new User(new CreateUserCommand(command.FullName, email, "", 0, "ACTIVE", owner.Id, true, true))
                { OrganizationId = organization.Id };
            user.SetPasswordHash(hasher.HashPassword(user, command.Password));
            relations.Add(user);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new AuthenticationResult(tokenProvider.Generate(user), user);
        }, cancellationToken);
    }

    public async Task<User> GetProfileAsync(CancellationToken cancellationToken = default)
    {
        await access.RequireAsync("read_users", cancellationToken);
        return await relations.Query<User>().SingleAsync(user => user.Id == request.UserId, cancellationToken);
    }

    public async Task<User> UpdateProfileAsync(UpdateProfileCommand command, CancellationToken cancellationToken = default)
    {
        await access.RequireAsync("read_users", cancellationToken);
        return await unitOfWork.ExecuteAsync(async () =>
        {
            var user = await GetProfileAsync(cancellationToken);
            if (command.Company is not null)
            {
                var organization = await relations.Query<Organization>().SingleAsync(item => item.Id == request.OrganizationId, cancellationToken);
                if (organization.Name != command.Company.Trim())
                {
                    await access.RequireAsync("manage_users", cancellationToken);
                    organization.Rename(command.Company);
                }
            }
            user.Update(new UpdateUserCommand(command.FullName, user.Email, command.Phone, command.Quota, user.Status, user.RoleId,
                user.CanExportData, user.CanDeleteLeads));
            return user;
        }, cancellationToken);
    }

    public async Task ChangePasswordAsync(ChangePasswordCommand command, CancellationToken cancellationToken = default)
    {
        var user = await GetProfileAsync(cancellationToken);
        if (command.NewPassword.Length is < 8 or > 128) throw new BusinessException("InvalidPassword");
        if (hasher.VerifyHashedPassword(user, user.PasswordHash, command.CurrentPassword) == PasswordVerificationResult.Failed)
            throw new BusinessException("InvalidCredentials", 401);
        user.SetPasswordHash(hasher.HashPassword(user, command.NewPassword));
        await unitOfWork.CompleteAsync(cancellationToken);
    }

    private static readonly string DummyHash = new PasswordHasher<User>().HashPassword(null!, "Unavailable-account-password");
}
