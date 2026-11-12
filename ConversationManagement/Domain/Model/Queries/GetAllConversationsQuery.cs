namespace NovaScale.Platform.API.ConversationManagement.Domain.Model.Queries;

public record GetAllConversationsQuery(string? Search = null, int Page = 1, int PageSize = 20, string? Status = null, string? Priority = null, long? UserId = null, long? TagId = null, long? ContactId = null);
