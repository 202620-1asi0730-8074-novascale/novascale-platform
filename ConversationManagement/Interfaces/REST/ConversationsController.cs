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
[Route("api/v1/conversations")]
public class ConversationsController(IConversationCommandService commandService, IConversationQueryService queryService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PageResult<ConversationResource>>> GetAll([FromQuery] string? search = null,
        [FromQuery] string? status = null, [FromQuery] string? priority = null, [FromQuery] [Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long? userId = null, [FromQuery] [Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long? tagId = null,
        [FromQuery] [Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long? contactId = null,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await queryService.Handle(new GetAllConversationsQuery(search, page, pageSize, status, priority, userId, tagId, contactId), cancellationToken);
        return Ok(result.Map(ConversationResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ConversationResource>> GetById([Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long id, CancellationToken cancellationToken)
    {
        var result = await queryService.Handle(new GetConversationByIdQuery(id), cancellationToken);
        return Ok(ConversationResourceFromEntityAssembler.ToResourceFromEntity(result));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<ConversationResource>> Create([FromBody] CreateConversationResource resource, CancellationToken cancellationToken)
    {
        var result = await commandService.Handle(CreateConversationCommandFromResourceAssembler.ToCommandFromResource(resource), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ConversationResourceFromEntityAssembler.ToResourceFromEntity(result));
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<ConversationResource>> Update([Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long id, [FromBody] UpdateConversationResource resource, CancellationToken cancellationToken)
    {
        var result = await commandService.Handle(id, UpdateConversationCommandFromResourceAssembler.ToCommandFromResource(resource), cancellationToken);
        return Ok(ConversationResourceFromEntityAssembler.ToResourceFromEntity(result));
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete([Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long id, CancellationToken cancellationToken)
    {
        await commandService.RemoveAsync(id, cancellationToken);
        return NoContent();
    }
}
