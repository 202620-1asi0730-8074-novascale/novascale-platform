namespace NovaScale.Platform.API.Shared.Domain.Model;

public abstract class Entity
{
    public long Id { get; protected set; }
    public long OrganizationId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
