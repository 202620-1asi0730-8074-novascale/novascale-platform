using NovaScale.Platform.API.Shared.Domain.Model;
using NovaScale.Platform.API.LeadManagement.Domain.Model.Commands;

namespace NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates;

public class Tag : Entity
{
    protected Tag() { }

    public Tag(CreateTagCommand command) => Update(new UpdateTagCommand(command.Name, command.Color));

    public string Name { get; private set; } = string.Empty;
    public string Color { get; private set; } = string.Empty;

    public void Update(UpdateTagCommand command)
    {
        Name = Guard.Text(command.Name, 50);
        Color = Guard.Text(command.Color, 20);
    }
}
