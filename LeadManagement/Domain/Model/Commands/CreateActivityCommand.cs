namespace NovaScale.Platform.API.LeadManagement.Domain.Model.Commands;

public record CreateActivityCommand(string Type, string Content, long UserId, long? ContactId);
