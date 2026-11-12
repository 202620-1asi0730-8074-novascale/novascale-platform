using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovaScale.Platform.API.OrganizationManagement.Application.Services;
using NovaScale.Platform.API.OrganizationManagement.Interfaces.REST.Resources;
using NovaScale.Platform.API.OrganizationManagement.Interfaces.REST.Transform;
using NovaScale.Platform.API.IdentityAccess.Domain.Model.Commands;

namespace NovaScale.Platform.API.OrganizationManagement.Interfaces.REST;

[Authorize]
[ApiController]
[Produces("application/json")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
[Route("api/v1/organizations")]
public class OrganizationsController(IOrganizationService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrganizationResource>>> GetAll(CancellationToken cancellationToken) =>
        Ok((await service.ListAsync(cancellationToken)).Select(OrganizationResourceFromEntityAssembler.ToResourceFromEntity));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<OrganizationResource>> Get([Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long id, CancellationToken cancellationToken) =>
        Ok(OrganizationResourceFromEntityAssembler.ToResourceFromEntity(await service.GetAsync(id, cancellationToken)));

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<ProvisionedOrganizationResource>> Create(CreateOrganizationResource resource, CancellationToken cancellationToken)
    {
        var result = await service.CreateAsync(new SignUpCommand(resource.OwnerName, resource.Name, resource.OwnerEmail, resource.OwnerPassword), cancellationToken);
        return StatusCode(201, new ProvisionedOrganizationResource(OrganizationResourceFromEntityAssembler.ToResourceFromEntity(result.Organization), result.OwnerToken));
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<OrganizationResource>> Update([Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long id, UpdateOrganizationResource resource, CancellationToken cancellationToken) =>
        Ok(OrganizationResourceFromEntityAssembler.ToResourceFromEntity(await service.UpdateAsync(id, resource.Name, cancellationToken)));

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete([Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long id, CancellationToken cancellationToken)
    {
        await service.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
