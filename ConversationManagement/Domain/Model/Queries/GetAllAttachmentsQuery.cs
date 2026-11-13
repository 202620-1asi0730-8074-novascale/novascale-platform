namespace NovaScale.Platform.API.ConversationManagement.Domain.Model.Queries;

public record GetAllAttachmentsQuery(string? Search, int Page, int PageSize, long? MessageId = null);
