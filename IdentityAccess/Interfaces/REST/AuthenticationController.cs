using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NovaScale.Platform.API.IdentityAccess.Application.Services;
using NovaScale.Platform.API.IdentityAccess.Domain.Model.Commands;
using NovaScale.Platform.API.IdentityAccess.Interfaces.REST.Resources;
using NovaScale.Platform.API.IdentityAccess.Interfaces.REST.Transform;

namespace NovaScale.Platform.API.IdentityAccess.Interfaces.REST;

[ApiController]
[Produces("application/json")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
[Route("api/v1/authentication")]
public class AuthenticationController(IAuthenticationService service) : ControllerBase
{
    [AllowAnonymous]
    [EnableRateLimiting("authentication")]
    [HttpPost("sign-in")]
    public async Task<ActionResult<AuthenticationResource>> SignIn(SignInResource resource, CancellationToken cancellationToken)
    {
        var result = await service.SignInAsync(new SignInCommand(resource.Email, resource.Password), cancellationToken);
        return Ok(new AuthenticationResource(result.Token, UserResourceFromEntityAssembler.ToResourceFromEntity(result.User)));
    }

    [AllowAnonymous]
    [EnableRateLimiting("authentication")]
    [HttpPost("sign-up")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<AuthenticationResource>> SignUp(SignUpResource resource, CancellationToken cancellationToken)
    {
        var result = await service.SignUpAsync(new SignUpCommand(resource.FullName, resource.Company, resource.Email, resource.Password), cancellationToken);
        return StatusCode(StatusCodes.Status201Created, new AuthenticationResource(result.Token, UserResourceFromEntityAssembler.ToResourceFromEntity(result.User)));
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<ActionResult<UserResource>> GetProfile(CancellationToken cancellationToken) =>
        Ok(UserResourceFromEntityAssembler.ToResourceFromEntity(await service.GetProfileAsync(cancellationToken)));

    [Authorize]
    [HttpPut("profile")]
    public async Task<ActionResult<UserResource>> UpdateProfile(UpdateProfileResource resource, CancellationToken cancellationToken) =>
        Ok(UserResourceFromEntityAssembler.ToResourceFromEntity(await service.UpdateProfileAsync(new UpdateProfileCommand(resource.FullName, resource.Phone, resource.Quota, resource.Company), cancellationToken)));

    [Authorize]
    [HttpPut("password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordResource resource, CancellationToken cancellationToken)
    {
        await service.ChangePasswordAsync(new ChangePasswordCommand(resource.CurrentPassword, resource.NewPassword), cancellationToken);
        return NoContent();
    }
}
