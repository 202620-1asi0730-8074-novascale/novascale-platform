namespace NovaScale.Platform.API.Shared.Application.Security;

public interface IRequestContext
{
    long UserId { get; }
    long OrganizationId { get; }
}
