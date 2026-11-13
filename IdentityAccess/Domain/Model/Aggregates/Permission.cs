using NovaScale.Platform.API.Shared.Domain.Model;
using NovaScale.Platform.API.IdentityAccess.Domain.Model.Commands;

namespace NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates;

public class Permission : Entity
{
    protected Permission() { }

    public Permission(CreatePermissionCommand command) => Update(new UpdatePermissionCommand(command.Code));

    public string Code { get; private set; } = string.Empty;

    public void Update(UpdatePermissionCommand command)
    {
        Code = Guard.Text(command.Code, 80);
    }
}
