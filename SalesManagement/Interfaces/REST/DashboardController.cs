using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovaScale.Platform.API.SalesManagement.Application.Services;

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
[Route("api/v1/dashboard")]
public class DashboardController(IDashboardQueryService service) : ControllerBase
{
    [HttpGet("summary")]
    public async Task<ActionResult<DashboardSummary>> GetSummary([FromQuery] DateOnly? from, [FromQuery] DateOnly? to,
        [FromQuery] [Range(1, long.MaxValue, ErrorMessage = "PositiveId")] long? userId, CancellationToken cancellationToken) => Ok(await service.GetAsync(from, to, userId, cancellationToken));

    [HttpGet("salespeople")]
    public async Task<ActionResult<IEnumerable<SalespersonSummary>>> GetSalespeople([FromQuery] DateOnly? from,
        [FromQuery] DateOnly? to, CancellationToken cancellationToken) => Ok(await service.GetSalespeopleAsync(from, to, cancellationToken));
}
