using System.ComponentModel.DataAnnotations;

namespace NovaScale.Platform.API.ConversationManagement.Interfaces.REST.Resources;

public record UpdateAttachmentResource(
    [Required(ErrorMessage = "RequiredField")] [StringLength(255, ErrorMessage = "LengthLimit")] string Name,
    [Required(ErrorMessage = "RequiredField")] [StringLength(100, ErrorMessage = "LengthLimit")] string ContentType,
    [Required(ErrorMessage = "RequiredField")] [StringLength(2800000, ErrorMessage = "LengthLimit")] string Data,
    [Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long MessageId);
