using System.ComponentModel.DataAnnotations;

namespace NovaScale.Platform.API.IdentityAccess.Interfaces.REST.Resources;

public record CreateRoleResource(
    [StringLength(50, ErrorMessage = "LengthLimit")]  [Required(ErrorMessage = "RequiredField")] string Name,
    [StringLength(255, ErrorMessage = "LengthLimit")] string? Description);
