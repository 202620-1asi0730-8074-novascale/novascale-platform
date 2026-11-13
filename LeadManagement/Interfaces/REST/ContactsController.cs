using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovaScale.Platform.API.LeadManagement.Application.Services;
using NovaScale.Platform.API.LeadManagement.Domain.Model.Queries;
using NovaScale.Platform.API.LeadManagement.Interfaces.REST.Resources;
using NovaScale.Platform.API.LeadManagement.Interfaces.REST.Transform;
using NovaScale.Platform.API.Shared.Application.Patterns;

namespace NovaScale.Platform.API.LeadManagement.Interfaces.REST;

[Authorize]
[ApiController]
[Produces("application/json")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
[Route("api/v1/contacts")]
public class ContactsController(IContactCommandService commandService, IContactQueryService queryService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PageResult<ContactResource>>> GetAll([FromQuery] string? search = null,
        [FromQuery] string? contactType = null, [FromQuery] string? status = null, [FromQuery] [Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long? userId = null, [FromQuery] [Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long? tagId = null,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await queryService.Handle(new GetAllContactsQuery(search, page, pageSize, contactType, status, userId, tagId), cancellationToken);
        var details = await queryService.GetDetailsAsync(result.Items.ToArray(), cancellationToken);
        return Ok(new PageResult<ContactResource>(details.Select(ContactResourceFromEntityAssembler.ToResourceFromDetails), result.Total, result.Page, result.PageSize));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ContactResource>> GetById([Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long id, CancellationToken cancellationToken)
    {
        var result = await queryService.Handle(new GetContactByIdQuery(id), cancellationToken);
        return Ok(ContactResourceFromEntityAssembler.ToResourceFromDetails((await queryService.GetDetailsAsync([result], cancellationToken))[0]));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<ContactResource>> Create([FromBody] CreateContactResource resource, CancellationToken cancellationToken)
    {
        var result = await commandService.Handle(CreateContactCommandFromResourceAssembler.ToCommandFromResource(resource), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ContactResourceFromEntityAssembler.ToResourceFromDetails((await queryService.GetDetailsAsync([result], cancellationToken))[0]));
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<ContactResource>> Update([Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long id, [FromBody] UpdateContactResource resource, CancellationToken cancellationToken)
    {
        var result = await commandService.Handle(id, UpdateContactCommandFromResourceAssembler.ToCommandFromResource(resource), cancellationToken);
        return Ok(ContactResourceFromEntityAssembler.ToResourceFromDetails((await queryService.GetDetailsAsync([result], cancellationToken))[0]));
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete([Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long id, CancellationToken cancellationToken)
    {
        await commandService.RemoveAsync(id, cancellationToken);
        return NoContent();
    }
}
