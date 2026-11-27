using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using NovaScale.Platform.API.Shared.Domain.Exceptions;
using NovaScale.Platform.API.ConversationManagement.Application.Services;
using NovaScale.Platform.API.ConversationManagement.Interfaces.REST.Transform;

namespace NovaScale.Platform.API.ConversationManagement.Interfaces.REST;

[Authorize]
[ApiController]
[Route("api/v1/integrations/gmail")]
public class GmailIntegrationController(IGmailIntegrationService service, ILogger<GmailIntegrationController> logger) : ControllerBase
{
    [HttpGet("status")]
    public async Task<IActionResult> Status(CancellationToken cancellationToken) => Ok(await service.StatusAsync(cancellationToken));

    [HttpPost("connect")]
    public async Task<IActionResult> Connect(CancellationToken cancellationToken) => Ok(new { url = await service.ConnectAsync(cancellationToken) });

    [AllowAnonymous]
    [HttpGet("callback")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> Callback([FromQuery] string? code, [FromQuery] string? state, CancellationToken cancellationToken)
    {
        Response.Headers["Referrer-Policy"] = "no-referrer";
        try
        {
            await service.CompleteConnectionAsync(code, state, cancellationToken);
            return Redirect(service.ReturnUrl(true));
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            var reason = exception is BusinessException business && business.Code is "GmailApiDisabled" or "GmailPermissionDenied" or "GmailReconnect" or "GmailAuthorizationFailed"
                ? business.Code : "GmailConnectionFailed";
            logger.LogWarning("Gmail authorization did not complete. Reason: {Reason}.", reason);
            return Redirect(QueryHelpers.AddQueryString(service.ReturnUrl(false), "gmailReason", reason));
        }
    }

    [HttpDelete("connection")]
    public async Task<IActionResult> Disconnect(CancellationToken cancellationToken)
    {
        await service.DisconnectAsync(cancellationToken);
        return NoContent();
    }

    [HttpPost("contacts/{contactId:long}/send")]
    public async Task<IActionResult> Send(long contactId, [FromBody] GmailSendRequest resource, CancellationToken cancellationToken)
    {
        var message = await service.SendAsync(contactId, resource, cancellationToken);
        return Ok(MessageResourceFromEntityAssembler.ToResourceFromEntity(message));
    }

    [HttpPost("contacts/{contactId:long}/sync")]
    public async Task<IActionResult> Sync(long contactId, CancellationToken cancellationToken) =>
        Ok(new { imported = await service.SyncAsync(contactId, cancellationToken) });
}
