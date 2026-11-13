using System.Buffers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NovaScale.Platform.API.Shared.Infrastructure.Interfaces.ASP.Configuration;

public sealed class MoneyJsonConverter : JsonConverter<decimal>
{
    public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.Number || !reader.TryGetDecimal(out var value))
            throw new JsonException("InvalidAmount");
        var text = Encoding.UTF8.GetString(reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan);
        var exponentIndex = text.IndexOfAny(['e', 'E']);
        var mantissa = exponentIndex >= 0 ? text[..exponentIndex] : text;
        var exponent = 0;
        if (exponentIndex >= 0 && !int.TryParse(text[(exponentIndex + 1)..], out exponent))
            throw new JsonException("InvalidAmount");
        var decimalIndex = mantissa.IndexOf('.');
        var decimals = decimalIndex >= 0 ? mantissa.Length - decimalIndex - 1 : 0;
        var digits = mantissa.Replace(".", "").TrimStart('-').TrimStart('0');
        var significant = digits.TrimEnd('0');
        if (significant.Length > 0 && (long)decimals - exponent - (digits.Length - significant.Length) > 2)
            throw new JsonException("InvalidAmount");
        return value;
    }

    public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options) => writer.WriteNumberValue(value);
}