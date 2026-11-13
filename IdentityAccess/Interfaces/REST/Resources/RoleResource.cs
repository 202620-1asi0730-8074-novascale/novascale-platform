namespace NovaScale.Platform.API.IdentityAccess.Interfaces.REST.Resources;

public record RoleResource(long Id, string Name, string Description, DateTime CreatedAt, DateTime UpdatedAt, string SystemCode);
