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
[Route("api/v1/roles")]
public class RolesController(IRoleCommandService commandService, IRoleQueryService queryService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PageResult<RoleResource>>> GetAll([FromQuery] string? search = null,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await queryService.Handle(new GetAllRolesQuery(search, page, pageSize), cancellationToken);
        return Ok(result.Map(RoleResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<RoleResource>> GetById([Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long id, CancellationToken cancellationToken)
    {
        var result = await queryService.Handle(new GetRoleByIdQuery(id), cancellationToken);
        return Ok(RoleResourceFromEntityAssembler.ToResourceFromEntity(result));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<RoleResource>> Create([FromBody] CreateRoleResource resource, CancellationToken cancellationToken)
    {
        var result = await commandService.Handle(CreateRoleCommandFromResourceAssembler.ToCommandFromResource(resource), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, RoleResourceFromEntityAssembler.ToResourceFromEntity(result));
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<RoleResource>> Update([Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long id, [FromBody] UpdateRoleResource resource, CancellationToken cancellationToken)
    {
        var result = await commandService.Handle(id, UpdateRoleCommandFromResourceAssembler.ToCommandFromResource(resource), cancellationToken);
        return Ok(RoleResourceFromEntityAssembler.ToResourceFromEntity(result));
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete([Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long id, CancellationToken cancellationToken)
    {
        await commandService.RemoveAsync(id, cancellationToken);
        return NoContent();
    }
}
