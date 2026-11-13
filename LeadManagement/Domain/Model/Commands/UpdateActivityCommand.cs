namespace NovaScale.Platform.API.LeadManagement.Domain.Model.Commands;

public record UpdateActivityCommand(string Type, string Content, long UserId, long? ContactId);
