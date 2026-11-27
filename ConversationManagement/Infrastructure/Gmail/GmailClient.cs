using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using NovaScale.Platform.API.Shared.Domain.Exceptions;

namespace NovaScale.Platform.API.ConversationManagement.Infrastructure.Gmail;

public sealed class GmailClient(HttpClient http, GmailOptions options)
{
    public async Task<JsonElement> ExchangeAsync(Dictionary<string, string> fields, CancellationToken cancellationToken)
    {
        fields["client_id"] = options.ClientId ?? throw new BusinessException("GmailNotConfigured", 503);
        fields["client_secret"] = options.ClientSecret ?? throw new BusinessException("GmailNotConfigured", 503);
        using var response = await http.PostAsync("https://oauth2.googleapis.com/token", new FormUrlEncodedContent(fields), cancellationToken);
        if (!response.IsSuccessStatusCode) throw new BusinessException("GmailReconnect", 409);
        using var document = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(cancellationToken), cancellationToken: cancellationToken);
        return document.RootElement.Clone();
    }

    public async Task<JsonElement> GetAsync(string token, string path, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://gmail.googleapis.com/gmail/v1/users/me/" + path);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        using var response = await http.SendAsync(request, cancellationToken);
        await CheckResponseAsync(response, cancellationToken);
        if (response.Content.Headers.ContentLength > 40 * 1024 * 1024) throw new BusinessException("GmailMessageTooLarge", 413);
        using var document = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(cancellationToken), cancellationToken: cancellationToken);
        return document.RootElement.Clone();
    }

    public async Task<JsonElement> SendAsync(string token, string raw, string? threadId, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "https://gmail.googleapis.com/gmail/v1/users/me/messages/send");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = JsonContent.Create(new { raw, threadId });
        using var response = await http.SendAsync(request, cancellationToken);
        await CheckResponseAsync(response, cancellationToken);
        using var document = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(cancellationToken), cancellationToken: cancellationToken);
        return document.RootElement.Clone();
    }

    private static async Task CheckResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var status = response.StatusCode;
        if ((int)status is >= 200 and < 300) return;
        if (status == HttpStatusCode.Forbidden)
        {
            try
            {
                using var document = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(cancellationToken), cancellationToken: cancellationToken);
                if (document.RootElement.TryGetProperty("error", out var error))
                {
                    var disabled = error.TryGetProperty("errors", out var errors) && errors.ValueKind == JsonValueKind.Array
                        && errors.EnumerateArray().Any(item => item.TryGetProperty("reason", out var reason) && reason.GetString() == "accessNotConfigured");
                    disabled |= error.TryGetProperty("details", out var details) && details.ValueKind == JsonValueKind.Array
                        && details.EnumerateArray().Any(item => item.TryGetProperty("reason", out var reason) && reason.GetString() == "SERVICE_DISABLED");
                    if (disabled) throw new BusinessException("GmailApiDisabled", 403);
                }
            }
            catch (JsonException) { }
        }
        throw status switch
        {
            HttpStatusCode.Unauthorized => new BusinessException("GmailReconnect", 409),
            HttpStatusCode.Forbidden => new BusinessException("GmailPermissionDenied", 403),
            HttpStatusCode.TooManyRequests => new BusinessException("GmailRateLimited", 429),
            HttpStatusCode.NotFound => new BusinessException("GmailMessageUnavailable", 404),
            HttpStatusCode.BadRequest => new BusinessException("GmailRequestRejected", 400),
            _ => new BusinessException("GmailUnavailable", 502)
        };
    }
}
