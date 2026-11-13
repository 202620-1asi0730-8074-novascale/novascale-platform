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
[Route("api/v1/opportunities")]
public class OpportunitiesController(IOpportunityCommandService commandService, IOpportunityQueryService queryService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PageResult<OpportunityResource>>> GetAll([FromQuery] string? search = null,
        [FromQuery] string? status = null, [FromQuery] [Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long? userId = null, [FromQuery] [Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long? contactId = null,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await queryService.Handle(new GetAllOpportunitiesQuery(search, page, pageSize, status, userId, contactId), cancellationToken);
        return Ok(result.Map(OpportunityResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<OpportunityResource>> GetById([Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long id, CancellationToken cancellationToken)
    {
        var result = await queryService.Handle(new GetOpportunityByIdQuery(id), cancellationToken);
        return Ok(OpportunityResourceFromEntityAssembler.ToResourceFromEntity(result));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<OpportunityResource>> Create([FromBody] CreateOpportunityResource resource, CancellationToken cancellationToken)
    {
        var result = await commandService.Handle(CreateOpportunityCommandFromResourceAssembler.ToCommandFromResource(resource), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, OpportunityResourceFromEntityAssembler.ToResourceFromEntity(result));
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<OpportunityResource>> Update([Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long id, [FromBody] UpdateOpportunityResource resource, CancellationToken cancellationToken)
    {
        var result = await commandService.Handle(id, UpdateOpportunityCommandFromResourceAssembler.ToCommandFromResource(resource), cancellationToken);
        return Ok(OpportunityResourceFromEntityAssembler.ToResourceFromEntity(result));
    }

    [HttpPatch("{id:long}/status")]
    public async Task<ActionResult<OpportunityResource>> ChangeStatus([Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long id, ChangeOpportunityStatusResource resource, CancellationToken cancellationToken)
    {
        var current = await queryService.Handle(new GetOpportunityByIdQuery(id), cancellationToken);
        var result = await commandService.Handle(id, new NovaScale.Platform.API.LeadManagement.Domain.Model.Commands.UpdateOpportunityCommand(
            current.Title, current.EstimatedAmount, resource.Status, current.ExpectedCloseDate, current.UserId, current.ContactId), cancellationToken);
        return Ok(OpportunityResourceFromEntityAssembler.ToResourceFromEntity(result));
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete([Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long id, CancellationToken cancellationToken)
    {
        await commandService.RemoveAsync(id, cancellationToken);
        return NoContent();
    }
}
