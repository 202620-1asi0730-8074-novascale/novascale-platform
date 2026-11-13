namespace NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates;

public class RolePermission
{
    public long OrganizationId { get; set; }
    public long RoleId { get; set; }
    public long PermissionId { get; set; }
}
