using Microsoft.AspNetCore.Localization;
using NovaScale.Platform.API.OrganizationManagement.Application.Services;
using NovaScale.Platform.API.OrganizationManagement.Application.Internal.Services;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using NovaScale.Platform.API.IdentityAccess.Domain.Services;
using NovaScale.Platform.API.IdentityAccess.Infrastructure.Security;
using NovaScale.Platform.API.Resources;
using NovaScale.Platform.API.Shared.Application.Security;
using NovaScale.Platform.API.Shared.Application.Services;
using NovaScale.Platform.API.Shared.Domain.Repositories;
using NovaScale.Platform.API.Shared.Infrastructure.Security;
using NovaScale.Platform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using NovaScale.Platform.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using NovaScale.Platform.API.Shared.Infrastructure.Interfaces.ASP.Errors;
using NovaScale.Platform.API.Shared.Infrastructure.Interfaces.ASP.Configuration;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.ConversationManagement.Domain.Repositories;
using NovaScale.Platform.API.ConversationManagement.Application.Services;
using NovaScale.Platform.API.ConversationManagement.Application.Internal.Services;
using NovaScale.Platform.API.ConversationManagement.Application.Internal.CommandServices;
using NovaScale.Platform.API.ConversationManagement.Application.Internal.QueryServices;
using NovaScale.Platform.API.ConversationManagement.Infrastructure.Persistence.EFC.Repositories;
using NovaScale.Platform.API.IdentityAccess.Domain.Model.Aggregates;
using NovaScale.Platform.API.IdentityAccess.Domain.Repositories;
using NovaScale.Platform.API.IdentityAccess.Application.Services;
using NovaScale.Platform.API.IdentityAccess.Application.Internal.Services;
using NovaScale.Platform.API.IdentityAccess.Application.Internal.CommandServices;
using NovaScale.Platform.API.IdentityAccess.Application.Internal.QueryServices;
using NovaScale.Platform.API.IdentityAccess.Infrastructure.Persistence.EFC.Repositories;
using NovaScale.Platform.API.LeadManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.LeadManagement.Domain.Repositories;
using NovaScale.Platform.API.LeadManagement.Application.Services;
using NovaScale.Platform.API.LeadManagement.Application.Internal.Services;
using NovaScale.Platform.API.LeadManagement.Application.Internal.CommandServices;
using NovaScale.Platform.API.LeadManagement.Application.Internal.QueryServices;
using NovaScale.Platform.API.LeadManagement.Infrastructure.Persistence.EFC.Repositories;
using NovaScale.Platform.API.SalesManagement.Domain.Model.Aggregates;
using NovaScale.Platform.API.SalesManagement.Domain.Repositories;
using NovaScale.Platform.API.SalesManagement.Application.Services;
using NovaScale.Platform.API.SalesManagement.Application.Internal.Services;
using NovaScale.Platform.API.SalesManagement.Application.Internal.CommandServices;
using NovaScale.Platform.API.SalesManagement.Application.Internal.QueryServices;
using NovaScale.Platform.API.SalesManagement.Infrastructure.Persistence.EFC.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options => options.Limits.MaxRequestBodySize = 9 * 1024 * 1024);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IRequestContext, RequestContext>();
builder.Services.AddScoped<IEntityRelations, EntityRelations>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAccessPolicy, AccessPolicy>();
builder.Services.AddScoped<EntityRules>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<ITokenProvider, TokenProvider>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<IDashboardQueryService, DashboardQueryService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IUserAccountService, UserAccountService>();
builder.Services.AddScoped<IMessageDeliveryService, MessageDeliveryService>();
builder.Services.AddScoped<IRolePermissionService, RolePermissionService>();
builder.Services.AddScoped<IContactTagService, ContactTagService>();
builder.Services.AddScoped<IConversationRepository, ConversationRepository>();
builder.Services.AddScoped<IConversationCommandService, ConversationCommandService>();
builder.Services.AddScoped<IConversationQueryService, ConversationQueryService>();
builder.Services.AddScoped<IEntityLifecycle<Conversation>, ConversationLifecycle>();
builder.Services.AddScoped<IAttachmentRepository, AttachmentRepository>();
builder.Services.AddScoped<IAttachmentCommandService, AttachmentCommandService>();
builder.Services.AddScoped<IAttachmentQueryService, AttachmentQueryService>();
builder.Services.AddScoped<IEntityLifecycle<Attachment>, AttachmentLifecycle>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IMessageCommandService, MessageCommandService>();
builder.Services.AddScoped<IMessageQueryService, MessageQueryService>();
builder.Services.AddScoped<IEntityLifecycle<Message>, MessageLifecycle>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationCommandService, NotificationCommandService>();
builder.Services.AddScoped<INotificationQueryService, NotificationQueryService>();
builder.Services.AddScoped<IEntityLifecycle<Notification>, NotificationLifecycle>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IPermissionCommandService, PermissionCommandService>();
builder.Services.AddScoped<IPermissionQueryService, PermissionQueryService>();
builder.Services.AddScoped<IEntityLifecycle<Permission>, PermissionLifecycle>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRoleCommandService, RoleCommandService>();
builder.Services.AddScoped<IRoleQueryService, RoleQueryService>();
builder.Services.AddScoped<IEntityLifecycle<Role>, RoleLifecycle>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserCommandService, UserCommandService>();
builder.Services.AddScoped<IUserQueryService, UserQueryService>();
builder.Services.AddScoped<IEntityLifecycle<User>, UserLifecycle>();
builder.Services.AddScoped<IActivityRepository, ActivityRepository>();
builder.Services.AddScoped<IActivityCommandService, ActivityCommandService>();
builder.Services.AddScoped<IActivityQueryService, ActivityQueryService>();
builder.Services.AddScoped<IEntityLifecycle<Activity>, ActivityLifecycle>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<IContactCommandService, ContactCommandService>();
builder.Services.AddScoped<IContactQueryService, ContactQueryService>();
builder.Services.AddScoped<IEntityLifecycle<Contact>, ContactLifecycle>();
builder.Services.AddScoped<IOpportunityRepository, OpportunityRepository>();
builder.Services.AddScoped<IOpportunityCommandService, OpportunityCommandService>();
builder.Services.AddScoped<IOpportunityQueryService, OpportunityQueryService>();
builder.Services.AddScoped<IEntityLifecycle<Opportunity>, OpportunityLifecycle>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<ITagCommandService, TagCommandService>();
builder.Services.AddScoped<ITagQueryService, TagQueryService>();
builder.Services.AddScoped<IEntityLifecycle<Tag>, TagLifecycle>();
builder.Services.AddScoped<ISaleRepository, SaleRepository>();
builder.Services.AddScoped<ISaleCommandService, SaleCommandService>();
builder.Services.AddScoped<ISaleQueryService, SaleQueryService>();
builder.Services.AddScoped<IEntityLifecycle<Sale>, SaleLifecycle>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connection = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrWhiteSpace(connection)) throw new InvalidOperationException("Configure ConnectionStrings:DefaultConnection using user secrets or environment variables.");
    options.UseMySQL(connection);
});

builder.Services.AddLocalization();
builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new MoneyJsonConverter())).AddDataAnnotationsLocalization(options =>
    options.DataAnnotationLocalizerProvider = (_, factory) => factory.Create(typeof(SharedResource)))
    .ConfigureApiBehaviorOptions(options => options.InvalidModelStateResponseFactory = LocalizedValidationResponseFactory.Create);
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddProblemDetails(options => options.CustomizeProblemDetails = context =>
{
    if (context.ProblemDetails.Extensions.ContainsKey("code")) return;
    var localizer = context.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<SharedResource>>();
    var code = context.ProblemDetails.Status switch
    {
        400 => "InvalidData", 401 => "Unauthorized", 403 => "Forbidden", 404 => "NotFound",
        405 => "MethodNotAllowed", 413 => "RequestTooLarge", 415 => "UnsupportedMediaType",
        429 => "TooManyRequests", _ => "UnexpectedError"
    };
    context.ProblemDetails.Title = localizer[code];
    context.ProblemDetails.Extensions["code"] = code;
});
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
    options.AddDocumentTransformer<LocalizedOpenApiTransformer>();
    options.AddOperationTransformer(LocalizedOpenApiTransformer.TransformOperationAsync);
});

var signingKey = builder.Configuration["Authentication:SigningKey"] ?? "";
if (Encoding.UTF8.GetByteCount(signingKey) < 32)
    throw new InvalidOperationException("Configure Authentication:SigningKey with at least 32 bytes using user secrets or environment variables.");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, ValidateAudience = true, ValidateLifetime = true, ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Authentication:Issuer"], ValidAudience = builder.Configuration["Authentication:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)), ClockSkew = TimeSpan.FromSeconds(30)
    };
});
builder.Services.AddAuthorization();
builder.Services.AddCors(options => options.AddPolicy("WebApp", policy =>
    policy.WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? []).AllowAnyHeader().AllowAnyMethod().WithExposedHeaders("Content-Language")));
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("authentication", context => RateLimitPartition.GetFixedWindowLimiter(
        context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        _ => new FixedWindowRateLimiterOptions { PermitLimit = 20, Window = TimeSpan.FromMinutes(1), QueueLimit = 0 }));
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    await using var scope = app.Services.CreateAsyncScope();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await context.Database.MigrateAsync();
}

string[] cultures = ["en", "es"];
var localization = new RequestLocalizationOptions().SetDefaultCulture("en")
    .AddSupportedCultures(cultures).AddSupportedUICultures(cultures);
localization.RequestCultureProviders = [new AcceptLanguageHeaderRequestCultureProvider()];
localization.ApplyCurrentCultureToResponseHeaders = true;
app.UseRequestLocalization(localization);
app.Use(async (context, next) =>
{
    context.Response.OnStarting(() =>
    {
        context.Response.Headers.ContentLanguage = context.Features.Get<IRequestCultureFeature>()?.RequestCulture.UICulture.Name ?? "en";
        context.Response.Headers.AppendCommaSeparatedValues("Vary", "Accept-Language");
        return Task.CompletedTask;
    });
    await next(context);
});
app.UseExceptionHandler();
app.UseStatusCodePages(async context =>
{
    var localizer = context.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<SharedResource>>();
    var code = context.HttpContext.Response.StatusCode switch { 401 => "Unauthorized", 403 => "Forbidden", 404 => "NotFound", 405 => "MethodNotAllowed", 413 => "RequestTooLarge", 415 => "UnsupportedMediaType", 429 => "TooManyRequests", _ => "UnexpectedError" };
    await context.HttpContext.Response.WriteAsJsonAsync(new ProblemDetails
    {
        Status = context.HttpContext.Response.StatusCode, Title = localizer[code], Extensions = { ["code"] = code }
    }, options: (System.Text.Json.JsonSerializerOptions?)null, contentType: "application/problem+json", cancellationToken: context.HttpContext.RequestAborted);
});
app.UseHttpsRedirection();
app.UseCors("WebApp");
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.MapOpenApi();
app.MapScalarApiReference();
app.MapControllers();
app.Run();

public partial class Program { }
