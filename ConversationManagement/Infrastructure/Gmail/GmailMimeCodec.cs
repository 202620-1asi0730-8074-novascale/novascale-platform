using System.Security.Cryptography;
using System.Text;
using MimeKit;
using MimeKit.Text;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.ValueObjects;
using NovaScale.Platform.API.Shared.Domain.Exceptions;

namespace NovaScale.Platform.API.ConversationManagement.Infrastructure.Gmail;

public static class GmailMimeCodec
{
    public static string Encode(byte[] bytes) => Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    public static byte[] Decode(string value) => Convert.FromBase64String(value.Replace('-', '+').Replace('_', '/').PadRight((value.Length + 3) / 4 * 4, '='));
    public static string ExternalId(string mailbox, string providerId) => "gmail:" + Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(mailbox.ToLowerInvariant())))[..16] + ":" + providerId;

    public static async Task<string> ComposeAsync(string mailbox, string recipient, string subject, string messageId,
        string content, IReadOnlyList<MessageAttachment> attachments, MimeMessage? parent, CancellationToken cancellationToken)
    {
        using var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(mailbox));
        message.To.Add(MailboxAddress.Parse(recipient));
        message.Subject = subject;
        message.MessageId = messageId;
        message.Date = DateTimeOffset.UtcNow;
        if (!string.IsNullOrWhiteSpace(parent?.MessageId))
        {
            message.InReplyTo = parent.MessageId;
            foreach (var reference in parent.References) message.References.Add(reference);
            message.References.Add(parent.MessageId);
        }
        var body = new BodyBuilder { TextBody = content };
        foreach (var attachment in attachments)
        {
            attachment.Validate();
            body.Attachments.Add(attachment.Name, Convert.FromBase64String(attachment.Data[(attachment.Data.IndexOf(',') + 1)..]), ContentType.Parse(attachment.ContentType));
        }
        message.Body = body.ToMessageBody();
        using var stream = new MemoryStream();
        await message.WriteToAsync(stream, cancellationToken);
        return Encode(stream.ToArray());
    }

    public static string Text(MimeMessage message)
    {
        var text = message.TextBody ?? (message.HtmlBody is { } html ? new HtmlToText().Convert(html) : "");
        text = new string(text.Where(character => !char.IsControl(character) || character is '\r' or '\n' or '\t').ToArray());
        return string.IsNullOrWhiteSpace(text) ? "[Email without text content]" : text.Length > 16000 ? text[..15970] + "\n[Content truncated]" : text;
    }

    public static IReadOnlyList<MessageAttachment> Attachments(MimeMessage message)
    {
        var files = new List<MessageAttachment>();
        foreach (var part in message.Attachments.OfType<MimePart>())
        {
            if (files.Count == 3) break;
            if (part.Content is null) continue;
            var type = part.ContentType.MimeType.ToLowerInvariant();
            if (type is not ("image/png" or "image/jpeg" or "image/webp" or "application/pdf" or "text/plain")) continue;
            using var stream = new MemoryStream();
            part.Content.DecodeTo(stream);
            if (stream.Length is 0 or > 2097152) continue;
            var fileName = Path.GetFileName((part.FileName ?? "attachment").Replace('\\', '/'));
            var file = new MessageAttachment(fileName, type, $"data:{type};base64," + Convert.ToBase64String(stream.ToArray()));
            try { file.Validate(); files.Add(file); }
            catch (BusinessException) { }
        }
        return files;
    }
}
