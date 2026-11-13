using System.ComponentModel.DataAnnotations;

namespace NovaScale.Platform.API.IdentityAccess.Interfaces.REST.Resources;

public record RolePermissionResource(long RoleId, long PermissionId);
public record AssignPermissionResource([Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long PermissionId);
