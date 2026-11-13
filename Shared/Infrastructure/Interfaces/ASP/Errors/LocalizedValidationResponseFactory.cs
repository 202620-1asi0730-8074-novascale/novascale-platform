using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Localization;
using NovaScale.Platform.API.Resources;

namespace NovaScale.Platform.API.Shared.Infrastructure.Interfaces.ASP.Errors;

public static class LocalizedValidationResponseFactory
{
    public static IActionResult Create(ActionContext context)
    {
        var localizer = context.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<SharedResource>>();
        var errors = context.ModelState.Where(entry => entry.Value?.Errors.Count > 0)
            .ToDictionary(entry => entry.Key, entry => entry.Value!.Errors
                .Select(error => Translate(error, entry.Key, localizer)).Distinct().ToArray());
        var result = new BadRequestObjectResult(new ValidationProblemDetails(errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = localizer["InvalidData"], Instance = context.HttpContext.Request.Path,
            Extensions = { ["code"] = "InvalidData" }
        });
        result.ContentTypes.Add("application/problem+json");
        return result;
    }

    private static string Translate(ModelError error, string field, IStringLocalizer<SharedResource> localizer)
    {
        string[] keys = ["RequiredField", "PositiveId", "InvalidEmail", "InvalidData", "InvalidPassword", "InvalidValue", "BodyRequired"];
        foreach (var key in keys)
            if (error.ErrorMessage == localizer[key].Value || error.ErrorMessage == key) return localizer[key];
        var lengthPattern = "^" + Regex.Escape(localizer["LengthLimit"].Value).Replace("\\{1}", "[0-9]+") + "$";
        if (Regex.IsMatch(error.ErrorMessage, lengthPattern)) return error.ErrorMessage;
        if (field.EndsWith("amount", StringComparison.OrdinalIgnoreCase) || field.EndsWith("quota", StringComparison.OrdinalIgnoreCase))
            return localizer["InvalidAmount"];
        if (field.EndsWith("date", StringComparison.OrdinalIgnoreCase) || field is "from" or "to") return localizer["InvalidDate"];
        return localizer[error.Exception is not null ? "InvalidJson" : "InvalidValue"];
    }
}