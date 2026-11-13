namespace NovaScale.Platform.API.LeadManagement.Interfaces.REST.Resources;

public record ActivityResource(long Id, string Type, string Content, long UserId, long? ContactId, DateTime CreatedAt, DateTime UpdatedAt);
