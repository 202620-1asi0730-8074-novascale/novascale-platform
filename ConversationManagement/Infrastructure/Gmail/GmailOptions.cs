using System.Text.Json;

namespace NovaScale.Platform.API.ConversationManagement.Infrastructure.Gmail;

public sealed class GmailOptions(IConfiguration configuration)
{
    public const string SendScope = "https://www.googleapis.com/auth/gmail.send";
    public const string ReadScope = "https://www.googleapis.com/auth/gmail.readonly";
    public string CallbackUrl => configuration["Gmail:CallbackUrl"] ?? "http://localhost:5285/api/v1/integrations/gmail/callback";
    public string FrontendUrl => configuration["Gmail:FrontendUrl"] ?? "http://localhost:5173/conversations";
    public string? ClientId => ReadCredential("client_id");
    public string? ClientSecret => ReadCredential("client_secret");
    public bool Configured => !string.IsNullOrWhiteSpace(ClientId) && !string.IsNullOrWhiteSpace(ClientSecret);

    private string? ReadCredential(string field)
    {
        var configured = configuration[field == "client_id" ? "Gmail:ClientId" : "Gmail:ClientSecret"];
        if (!string.IsNullOrWhiteSpace(configured)) return configured;
        var path = configuration["Gmail:CredentialsPath"];
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path)) return null;
        using var document = JsonDocument.Parse(File.ReadAllText(path));
        return document.RootElement.TryGetProperty("web", out var web) && web.TryGetProperty(field, out var value)
            ? value.GetString() : null;
    }
}
