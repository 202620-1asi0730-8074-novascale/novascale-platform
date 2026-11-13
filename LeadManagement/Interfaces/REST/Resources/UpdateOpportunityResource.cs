using System.ComponentModel.DataAnnotations;

namespace NovaScale.Platform.API.LeadManagement.Interfaces.REST.Resources;

public record UpdateOpportunityResource(
    [StringLength(150, ErrorMessage = "LengthLimit")]  [Required(ErrorMessage = "RequiredField")] string Title,
    [property: System.Text.Json.Serialization.JsonRequired] decimal EstimatedAmount,
    [StringLength(120, ErrorMessage = "LengthLimit")]  [Required(ErrorMessage = "RequiredField")] string Status,
    DateOnly? ExpectedCloseDate,
    [Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long UserId,
    [Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long ContactId);
