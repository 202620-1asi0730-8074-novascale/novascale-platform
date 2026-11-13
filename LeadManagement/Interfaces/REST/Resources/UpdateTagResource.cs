using System.ComponentModel.DataAnnotations;

namespace NovaScale.Platform.API.LeadManagement.Interfaces.REST.Resources;

public record UpdateTagResource(
    [StringLength(50, ErrorMessage = "LengthLimit")]  [Required(ErrorMessage = "RequiredField")] string Name,
    [StringLength(20, ErrorMessage = "LengthLimit")]  [Required(ErrorMessage = "RequiredField")] string Color);
