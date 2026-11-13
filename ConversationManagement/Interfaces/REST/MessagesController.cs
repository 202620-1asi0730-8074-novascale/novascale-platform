using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovaScale.Platform.API.ConversationManagement.Application.Services;
using NovaScale.Platform.API.ConversationManagement.Domain.Model.Queries;
using NovaScale.Platform.API.ConversationManagement.Interfaces.REST.Resources;
using NovaScale.Platform.API.ConversationManagement.Interfaces.REST.Transform;
using NovaScale.Platform.API.Shared.Application.Patterns;

namespace NovaScale.Platform.API.ConversationManagement.Interfaces.REST;

[Authorize]
[ApiController]
[Produces("application/json")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
[Route("api/v1/messages")]
public class MessagesController(IMessageCommandService commandService, IMessageQueryService queryService, IMessageDeliveryService deliveryService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PageResult<MessageResource>>> GetAll([FromQuery] string? search = null,
        [FromQuery] [Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long? conversationId = null,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await queryService.Handle(new GetAllMessagesQuery(search, page, pageSize, conversationId), cancellationToken);
        return Ok(result.Map(MessageResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<MessageResource>> GetById([Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long id, CancellationToken cancellationToken)
    {
        var result = await queryService.Handle(new GetMessageByIdQuery(id), cancellationToken);
        return Ok(MessageResourceFromEntityAssembler.ToResourceFromEntity(result));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<MessageResource>> Create([FromBody] CreateMessageResource resource, CancellationToken cancellationToken)
    {
        var result = await deliveryService.CreateAsync(CreateMessageCommandFromResourceAssembler.ToCommandFromResource(resource), resource.Attachments, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, MessageResourceFromEntityAssembler.ToResourceFromEntity(result));
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<MessageResource>> Update([Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long id, [FromBody] UpdateMessageResource resource, CancellationToken cancellationToken)
    {
        var result = await deliveryService.UpdateAsync(id, UpdateMessageCommandFromResourceAssembler.ToCommandFromResource(resource), resource.Attachments, cancellationToken);
        return Ok(MessageResourceFromEntityAssembler.ToResourceFromEntity(result));
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete([Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long id, CancellationToken cancellationToken)
    {
        await commandService.RemoveAsync(id, cancellationToken);
        return NoContent();
    }
}
