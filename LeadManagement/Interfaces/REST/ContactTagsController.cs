using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovaScale.Platform.API.LeadManagement.Application.Services;
using NovaScale.Platform.API.LeadManagement.Interfaces.REST.Resources;

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
[Route("api/v1/contacts/{parentId:long}/tags")]
public class ContactTagsController(IContactTagService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ContactTagResource>>> GetAll([Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long parentId, CancellationToken cancellationToken) =>
        Ok((await service.ListAsync(parentId, cancellationToken)).Select(item => new ContactTagResource(item.ContactId, item.TagId)));

    [HttpGet("{targetId:long}")]
    public async Task<ActionResult<ContactTagResource>> Get([Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long parentId, [Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long targetId, CancellationToken cancellationToken)
    {
        var item = await service.GetAsync(parentId, targetId, cancellationToken);
        return Ok(new ContactTagResource(item.ContactId, item.TagId));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<ContactTagResource>> Assign([Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long parentId, AssignTagResource resource, CancellationToken cancellationToken)
    {
        var item = await service.AssignAsync(parentId, resource.TagId, cancellationToken);
        return CreatedAtAction(nameof(Get), new { parentId, targetId = item.TagId }, new ContactTagResource(item.ContactId, item.TagId));
    }

    [HttpPut("{targetId:long}")]
    public async Task<ActionResult<ContactTagResource>> Replace([Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long parentId, [Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long targetId, AssignTagResource resource, CancellationToken cancellationToken)
    {
        var item = await service.ReplaceAsync(parentId, targetId, resource.TagId, cancellationToken);
        return Ok(new ContactTagResource(item.ContactId, item.TagId));
    }

    [HttpDelete("{targetId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Remove([Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long parentId, [Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long targetId, CancellationToken cancellationToken)
    {
        await service.RemoveAsync(parentId, targetId, cancellationToken);
        return NoContent();
    }
}
