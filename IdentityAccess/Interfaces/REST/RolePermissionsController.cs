using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovaScale.Platform.API.IdentityAccess.Application.Services;
using NovaScale.Platform.API.IdentityAccess.Interfaces.REST.Resources;

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
[Route("api/v1/roles/{parentId:long}/permissions")]
public class RolePermissionsController(IRolePermissionService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RolePermissionResource>>> GetAll([Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long parentId, CancellationToken cancellationToken) =>
        Ok((await service.ListAsync(parentId, cancellationToken)).Select(item => new RolePermissionResource(item.RoleId, item.PermissionId)));

    [HttpGet("{targetId:long}")]
    public async Task<ActionResult<RolePermissionResource>> Get([Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long parentId, [Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long targetId, CancellationToken cancellationToken)
    {
        var item = await service.GetAsync(parentId, targetId, cancellationToken);
        return Ok(new RolePermissionResource(item.RoleId, item.PermissionId));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<RolePermissionResource>> Assign([Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long parentId, AssignPermissionResource resource, CancellationToken cancellationToken)
    {
        var item = await service.AssignAsync(parentId, resource.PermissionId, cancellationToken);
        return CreatedAtAction(nameof(Get), new { parentId, targetId = item.PermissionId }, new RolePermissionResource(item.RoleId, item.PermissionId));
    }

    [HttpPut("{targetId:long}")]
    public async Task<ActionResult<RolePermissionResource>> Replace([Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long parentId, [Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long targetId, AssignPermissionResource resource, CancellationToken cancellationToken)
    {
        var item = await service.ReplaceAsync(parentId, targetId, resource.PermissionId, cancellationToken);
        return Ok(new RolePermissionResource(item.RoleId, item.PermissionId));
    }

    [HttpDelete("{targetId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Remove([Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long parentId, [Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long targetId, CancellationToken cancellationToken)
    {
        await service.RemoveAsync(parentId, targetId, cancellationToken);
        return NoContent();
    }
}
