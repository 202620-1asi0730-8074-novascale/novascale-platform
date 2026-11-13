using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovaScale.Platform.API.IdentityAccess.Application.Services;
using NovaScale.Platform.API.IdentityAccess.Domain.Model.Queries;
using NovaScale.Platform.API.IdentityAccess.Interfaces.REST.Resources;
using NovaScale.Platform.API.IdentityAccess.Interfaces.REST.Transform;
using NovaScale.Platform.API.Shared.Application.Patterns;

namespace NovaScale.Platform.API.IdentityAccess.Interfaces.REST;

[Authorize]
[ApiController]
[Produces("application/json")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
[Route("api/v1/permissions")]
public class PermissionsController(IPermissionCommandService commandService, IPermissionQueryService queryService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PageResult<PermissionResource>>> GetAll([FromQuery] string? search = null,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await queryService.Handle(new GetAllPermissionsQuery(search, page, pageSize), cancellationToken);
        return Ok(result.Map(PermissionResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<PermissionResource>> GetById([Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long id, CancellationToken cancellationToken)
    {
        var result = await queryService.Handle(new GetPermissionByIdQuery(id), cancellationToken);
        return Ok(PermissionResourceFromEntityAssembler.ToResourceFromEntity(result));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<PermissionResource>> Create([FromBody] CreatePermissionResource resource, CancellationToken cancellationToken)
    {
        var result = await commandService.Handle(CreatePermissionCommandFromResourceAssembler.ToCommandFromResource(resource), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, PermissionResourceFromEntityAssembler.ToResourceFromEntity(result));
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<PermissionResource>> Update([Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long id, [FromBody] UpdatePermissionResource resource, CancellationToken cancellationToken)
    {
        var result = await commandService.Handle(id, UpdatePermissionCommandFromResourceAssembler.ToCommandFromResource(resource), cancellationToken);
        return Ok(PermissionResourceFromEntityAssembler.ToResourceFromEntity(result));
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete([Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long id, CancellationToken cancellationToken)
    {
        await commandService.RemoveAsync(id, cancellationToken);
        return NoContent();
    }
}
