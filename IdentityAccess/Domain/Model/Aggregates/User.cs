using NovaScale.Platform.API.Shared.Domain.Model;
using NovaScale.Platform.API.IdentityAccess.Domain.Model.Commands;

namespace NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates;

public class User : Entity
{
    protected User() { }

    public User(CreateUserCommand command) => Update(new UpdateUserCommand(command.FullName, command.Email, command.Phone, command.Quota, command.Status, command.RoleId, command.CanExportData, command.CanDeleteLeads));

    public string FullName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public decimal Quota { get; private set; }
    public string Status { get; private set; } = string.Empty;
    public long RoleId { get; private set; }
    public bool CanExportData { get; private set; }
    public bool CanDeleteLeads { get; private set; }

    public string PasswordHash { get; private set; } = string.Empty;
    public void SetPasswordHash(string hash) => PasswordHash = hash;

    public void Update(UpdateUserCommand command)
    {
        FullName = Guard.Text(command.FullName, 120);
        Email = Guard.Email(command.Email);
        Phone = Guard.Phone(command.Phone);
        Quota = Guard.Amount(command.Quota);
        Status = Guard.Choice(command.Status, "ACTIVE", "INACTIVE");
        RoleId = command.RoleId;
        CanExportData = command.CanExportData;
        CanDeleteLeads = command.CanDeleteLeads;
    }
}
