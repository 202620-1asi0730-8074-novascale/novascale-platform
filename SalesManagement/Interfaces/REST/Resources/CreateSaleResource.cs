using System.ComponentModel.DataAnnotations;

namespace NovaScale.Platform.API.SalesManagement.Interfaces.REST.Resources;

public record CreateSaleResource(
    [property: System.Text.Json.Serialization.JsonRequired] decimal Amount,
    DateOnly SaleDate,
    [StringLength(120, ErrorMessage = "LengthLimit")]  [Required(ErrorMessage = "RequiredField")] string Status,
    [StringLength(4000, ErrorMessage = "LengthLimit")] string? Notes,
    [Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long UserId,
    [Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long OpportunityId);
