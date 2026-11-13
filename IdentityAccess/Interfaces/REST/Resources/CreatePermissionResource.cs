using System.ComponentModel.DataAnnotations;

namespace NovaScale.Platform.API.IdentityAccess.Interfaces.REST.Resources;

public record CreatePermissionResource(
    [StringLength(80, ErrorMessage = "LengthLimit")]  [Required(ErrorMessage = "RequiredField")] string Code);
