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
[Route("api/v1/message-attachments")]
public class AttachmentsController(IAttachmentCommandService commandService, IAttachmentQueryService queryService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PageResult<AttachmentResource>>> GetAll([FromQuery] string? search = null,
        [FromQuery] [Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long? messageId = null,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await queryService.Handle(new GetAllAttachmentsQuery(search, page, pageSize, messageId), cancellationToken);
        return Ok(result.Map(AttachmentResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<AttachmentResource>> GetById([Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long id, CancellationToken cancellationToken)
    {
        var result = await queryService.Handle(new GetAttachmentByIdQuery(id), cancellationToken);
        return Ok(AttachmentResourceFromEntityAssembler.ToResourceFromEntity(result));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<AttachmentResource>> Create([FromBody] CreateAttachmentResource resource, CancellationToken cancellationToken)
    {
        var result = await commandService.Handle(CreateAttachmentCommandFromResourceAssembler.ToCommandFromResource(resource), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, AttachmentResourceFromEntityAssembler.ToResourceFromEntity(result));
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<AttachmentResource>> Update([Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long id, [FromBody] UpdateAttachmentResource resource, CancellationToken cancellationToken)
    {
        var result = await commandService.Handle(id, UpdateAttachmentCommandFromResourceAssembler.ToCommandFromResource(resource), cancellationToken);
        return Ok(AttachmentResourceFromEntityAssembler.ToResourceFromEntity(result));
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete([Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long id, CancellationToken cancellationToken)
    {
        await commandService.RemoveAsync(id, cancellationToken);
        return NoContent();
    }
}
