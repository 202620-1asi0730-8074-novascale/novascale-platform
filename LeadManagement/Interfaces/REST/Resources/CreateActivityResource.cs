using System.ComponentModel.DataAnnotations;

namespace NovaScale.Platform.API.LeadManagement.Interfaces.REST.Resources;

public record CreateActivityResource(
    [StringLength(40, ErrorMessage = "LengthLimit")]  [Required(ErrorMessage = "RequiredField")] string Type,
    [StringLength(1000, ErrorMessage = "LengthLimit")]  [Required(ErrorMessage = "RequiredField")] string Content,
    [Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long UserId,
    long? ContactId);
