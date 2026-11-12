using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Localization;
using Microsoft.OpenApi;
using NovaScale.Platform.API.Resources;

namespace NovaScale.Platform.API.Shared.Infrastructure.Interfaces.ASP.Configuration;

public sealed class LocalizedOpenApiTransformer(IStringLocalizer<SharedResource> localizer) : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Info.Title = localizer["ApiTitle"];
        document.Info.Description = localizer["ApiDescription"];
        return Task.CompletedTask;
    }

    public static Task TransformOperationAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        var strings = context.ApplicationServices.GetRequiredService<IStringLocalizer<SharedResource>>();
        if (context.Description.ActionDescriptor is ControllerActionDescriptor action)
        {
            var resource = strings["ApiResource" + action.ControllerName];
            var actionKey = "ApiAction" + action.ActionName;
            var text = strings[actionKey, resource.Value];
            operation.Summary = text.ResourceNotFound ? strings["ApiActionDefault", resource.Value] : text.Value;
        }
        operation.Parameters ??= [];
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "Accept-Language", In = ParameterLocation.Header, Description = strings["ApiLanguage"],
            Schema = new OpenApiSchema { Type = JsonSchemaType.String, Default = System.Text.Json.Nodes.JsonValue.Create("en") }
        });
        if (operation.Responses is not null)
            foreach (var (status, response) in operation.Responses)
                if (response is OpenApiResponse concrete)
                {
                    var description = strings["ApiResponse" + status];
                    if (!description.ResourceNotFound) concrete.Description = description.Value;
                }
        return Task.CompletedTask;
    }
}