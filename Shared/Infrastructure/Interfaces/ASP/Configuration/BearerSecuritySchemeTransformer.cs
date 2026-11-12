using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace NovaScale.Platform.API.Shared.Infrastructure.Interfaces.ASP.Configuration;

public sealed class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http, Scheme = "bearer", BearerFormat = "JWT"
        };
        foreach (var path in document.Paths)
        {
            if (path.Value.Operations is null) continue;
            foreach (var operation in path.Value.Operations.Values)
                operation.Security = path.Key.EndsWith("/sign-in") || path.Key.EndsWith("/sign-up")
                    ? [] : [new OpenApiSecurityRequirement { [new OpenApiSecuritySchemeReference("Bearer", document)] = [] }];
        }
        return Task.CompletedTask;
    }
}
