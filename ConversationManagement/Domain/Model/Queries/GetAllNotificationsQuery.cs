namespace NovaScale.Platform.API.ConversationManagement.Domain.Model.Queries;

public record GetAllNotificationsQuery(string? Search = null, int Page = 1, int PageSize = 20, bool? IsRead = null, long? ConversationId = null);
