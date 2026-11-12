using System.ComponentModel.DataAnnotations;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.ValueObjects;

namespace NovaScale.Platform.API.ConversationManagement.Interfaces.REST.Resources;

public record UpdateMessageResource(
    [StringLength(120, ErrorMessage = "LengthLimit")]  [Required(ErrorMessage = "RequiredField")] string SenderType,
    [StringLength(16000, ErrorMessage = "LengthLimit")] string? Content,
    [Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long ConversationId,
    IReadOnlyList<MessageAttachment>? Attachments = null);
