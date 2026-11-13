using NovaScale.Platform.API.Shared.Domain.Model;

namespace NovaScale.Platform.API.OrganizationManagement.Domain.Model.Aggregates;

public class Organization
{
    protected Organization() { }
    public Organization(string name) => Rename(name);
    public long Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public void Rename(string name) => Name = Guard.Text(name, 120);
}
