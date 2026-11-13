namespace NovaScale.Platform.API.ConversationManagement.Domain.Model.Queries;

public record GetAllMessagesQuery(string? Search = null, int Page = 1, int PageSize = 20, long? ConversationId = null);
