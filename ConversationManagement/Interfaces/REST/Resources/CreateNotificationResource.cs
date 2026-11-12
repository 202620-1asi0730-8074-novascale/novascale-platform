using System.ComponentModel.DataAnnotations;

namespace NovaScale.Platform.API.ConversationManagement.Interfaces.REST.Resources;

public record CreateNotificationResource(
    [StringLength(120, ErrorMessage = "LengthLimit")]  [Required(ErrorMessage = "RequiredField")] string Type,
    [StringLength(255, ErrorMessage = "LengthLimit")]  [Required(ErrorMessage = "RequiredField")] string Content,
    bool IsRead,
    [Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long UserId,
    [Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long ConversationId);
