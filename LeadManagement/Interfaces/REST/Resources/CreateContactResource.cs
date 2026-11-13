using System.ComponentModel.DataAnnotations;

namespace NovaScale.Platform.API.LeadManagement.Interfaces.REST.Resources;

public record CreateContactResource(
    [StringLength(120, ErrorMessage = "LengthLimit")]  [Required(ErrorMessage = "RequiredField")] string FullName,
    [StringLength(120, ErrorMessage = "LengthLimit")] string? Email,
    [StringLength(30, ErrorMessage = "LengthLimit")] string? Phone,
    [StringLength(120, ErrorMessage = "LengthLimit")] string? Company,
    [StringLength(120, ErrorMessage = "LengthLimit")]  [Required(ErrorMessage = "RequiredField")] string ContactType,
    [StringLength(120, ErrorMessage = "LengthLimit")]  [Required(ErrorMessage = "RequiredField")] string LeadStatus,
    [StringLength(80, ErrorMessage = "LengthLimit")] string? Source,
    [Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long UserId,
    decimal? EstimatedAmount = null,
    IReadOnlyList<string>? Tags = null);
