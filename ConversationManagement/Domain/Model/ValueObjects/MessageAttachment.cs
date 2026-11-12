using NovaScale.Platform.API.Shared.Domain.Exceptions;

namespace NovaScale.Platform.API.ConversationManagement.Domain.Model.ValueObjects;

public record MessageAttachment(string Name, string ContentType, string Data)
{
    public int Validate()
    {
        string[] types = ["image/png", "image/jpeg", "image/webp", "application/pdf", "text/plain"];
        if (string.IsNullOrWhiteSpace(Name) || Name.Length > 255 || Name is "." or ".."
            || Name.Any(character => char.IsControl(character) || "/\\".Contains(character))
            || !types.Contains(ContentType)
            || Data is null || !Data.StartsWith($"data:{ContentType};base64,", StringComparison.Ordinal)
            || Data.Length > 2800000) throw new BusinessException("InvalidAttachment");
        try
        {
            var bytes = Convert.FromBase64String(Data[(Data.IndexOf(',') + 1)..]);
            if (bytes.Length is 0 or > 2097152) throw new BusinessException("InvalidAttachment");
            var matches = ContentType switch
            {
                "image/png" => bytes.AsSpan().StartsWith(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }),
                "image/jpeg" => bytes.AsSpan().StartsWith(new byte[] { 255, 216, 255 }),
                "image/webp" => bytes.Length >= 12 && bytes.AsSpan(0, 4).SequenceEqual("RIFF"u8) && bytes.AsSpan(8, 4).SequenceEqual("WEBP"u8),
                "application/pdf" => bytes.AsSpan().StartsWith("%PDF-"u8),
                "text/plain" => IsValidText(bytes),
                _ => false
            };
            if (!matches) throw new BusinessException("InvalidAttachment");
            return bytes.Length;
        }
        catch (FormatException) { throw new BusinessException("InvalidAttachment"); }
    }
    private static bool IsValidText(byte[] bytes)
    {
        try
        {
            var text = new System.Text.UTF8Encoding(false, true).GetString(bytes);
            return !text.Any(character => char.IsControl(character) && character is not '\t' and not '\r' and not '\n');
        }
        catch (System.Text.DecoderFallbackException) { return false; }
    }

}
