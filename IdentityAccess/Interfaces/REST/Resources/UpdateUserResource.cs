using System.ComponentModel.DataAnnotations;

namespace NovaScale.Platform.API.IdentityAccess.Interfaces.REST.Resources;

public record UpdateUserResource(
    [StringLength(120, ErrorMessage = "LengthLimit")]  [Required(ErrorMessage = "RequiredField")] string FullName,
    [StringLength(120, ErrorMessage = "LengthLimit")]  [Required(ErrorMessage = "RequiredField")] [EmailAddress(ErrorMessage = "InvalidEmail")] string Email,
    [StringLength(30, ErrorMessage = "LengthLimit")] string? Phone,
    [property: System.Text.Json.Serialization.JsonRequired] decimal Quota,
    [StringLength(120, ErrorMessage = "LengthLimit")]  [Required(ErrorMessage = "RequiredField")] string Status,
    [Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long RoleId,
    bool CanExportData,
    bool CanDeleteLeads,
    [StringLength(128, MinimumLength = 8, ErrorMessage = "InvalidPassword")] string? Password = null);
