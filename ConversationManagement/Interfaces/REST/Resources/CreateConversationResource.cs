using System.ComponentModel.DataAnnotations;

namespace NovaScale.Platform.API.ConversationManagement.Interfaces.REST.Resources;

public record CreateConversationResource(
    [StringLength(120, ErrorMessage = "LengthLimit")]  [Required(ErrorMessage = "RequiredField")] string Channel,
    [StringLength(120, ErrorMessage = "LengthLimit")]  [Required(ErrorMessage = "RequiredField")] string Status,
    [Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long ContactId,
    [Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long UserId);
