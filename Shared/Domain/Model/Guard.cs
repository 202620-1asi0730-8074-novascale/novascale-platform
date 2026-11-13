using System.Globalization;
using NovaScale.Platform.API.Shared.Domain.Exceptions;

namespace NovaScale.Platform.API.Shared.Domain.Model;

public static class Guard
{
    public static string Text(string? value, int maximum, bool required = true)
    {
        var result = value?.Trim() ?? string.Empty;
        if ((required && !result.Any(character => !char.IsWhiteSpace(character) && !char.IsControl(character)
                && char.GetUnicodeCategory(character) is not UnicodeCategory.Format and not UnicodeCategory.NonSpacingMark
                    and not UnicodeCategory.EnclosingMark and not UnicodeCategory.SpacingCombiningMark))
            || result.Any(character => char.IsControl(character) && character is not '\t' and not '\r' and not '\n')
            || result.Length > maximum)
            throw new BusinessException("InvalidData");
        return result;
    }

    public static string Choice(string value, params string[] allowed)
    {
        var result = value.Trim().ToUpperInvariant();
        if (!allowed.Contains(result)) throw new BusinessException("InvalidState");
        return result;
    }

    public static string Email(string value, bool required = true)
    {
        var result = Text(value, 120, required).ToLowerInvariant();
        if (result.Length > 0 && (!System.Net.Mail.MailAddress.TryCreate(result, out var address) || address.Address != result))
            throw new BusinessException("InvalidEmail");
        return result;
    }

    public static string Phone(string? value)
    {
        if (value is not null && value.Any(character => !char.IsDigit(character) && !"+ ()-.".Contains(character)))
            throw new BusinessException("InvalidData");
        var result = new string((value ?? string.Empty).Where(character => char.IsDigit(character) || character == '+').ToArray());
        if (result.Length > 0 && !System.Text.RegularExpressions.Regex.IsMatch(result, @"^\+?[0-9]{7,15}$"))
            throw new BusinessException("InvalidData");
        return result;
    }

    public static DateOnly Date(DateOnly value)
    {
        if (value.Year < 1000) throw new BusinessException("InvalidData");
        return value;
    }

    public static decimal Amount(decimal value)
    {
        if (value < 0 || value > 9999999999.99m || decimal.Round(value, 2) != value)
            throw new BusinessException("InvalidAmount");
        return value;
    }
}
