using NovaScale.Platform.API.Shared.Domain.Model;
using NovaScale.Platform.API.IdentityAccess.Domain.Model.Commands;

namespace NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates;

public class Role : Entity
{
    protected Role() { }

    public Role(CreateRoleCommand command) => Update(new UpdateRoleCommand(command.Name, command.Description));

    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    public string SystemCode { get; private set; } = string.Empty;
    public void SetSystemCode(string code) => SystemCode = code;

    public void Update(UpdateRoleCommand command)
    {
        Name = Guard.Text(command.Name, 50);
        Description = Guard.Text(command.Description, 255, false);
    }
}
