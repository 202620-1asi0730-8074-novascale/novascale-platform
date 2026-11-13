using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovaScale.Platform.API.SalesManagement.Application.Services;
using NovaScale.Platform.API.SalesManagement.Domain.Model.Queries;
using NovaScale.Platform.API.SalesManagement.Interfaces.REST.Resources;
using NovaScale.Platform.API.SalesManagement.Interfaces.REST.Transform;
using NovaScale.Platform.API.Shared.Application.Patterns;

namespace NovaScale.Platform.API.SalesManagement.Interfaces.REST;

[Authorize]
[ApiController]
[Produces("application/json")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
[Route("api/v1/sales")]
public class SalesController(ISaleCommandService commandService, ISaleQueryService queryService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PageResult<SaleResource>>> GetAll([FromQuery] string? search = null,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await queryService.Handle(new GetAllSalesQuery(search, page, pageSize), cancellationToken);
        return Ok(result.Map(SaleResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<SaleResource>> GetById([Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long id, CancellationToken cancellationToken)
    {
        var result = await queryService.Handle(new GetSaleByIdQuery(id), cancellationToken);
        return Ok(SaleResourceFromEntityAssembler.ToResourceFromEntity(result));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<SaleResource>> Create([FromBody] CreateSaleResource resource, CancellationToken cancellationToken)
    {
        var result = await commandService.Handle(CreateSaleCommandFromResourceAssembler.ToCommandFromResource(resource), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, SaleResourceFromEntityAssembler.ToResourceFromEntity(result));
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<SaleResource>> Update([Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long id, [FromBody] UpdateSaleResource resource, CancellationToken cancellationToken)
    {
        var result = await commandService.Handle(id, UpdateSaleCommandFromResourceAssembler.ToCommandFromResource(resource), cancellationToken);
        return Ok(SaleResourceFromEntityAssembler.ToResourceFromEntity(result));
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete([Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long id, CancellationToken cancellationToken)
    {
        await commandService.RemoveAsync(id, cancellationToken);
        return NoContent();
    }
}
