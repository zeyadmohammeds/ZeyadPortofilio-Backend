using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Portfolio.Application.Auth;
using Portfolio.Application.Common;

namespace Portfolio.API.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
public sealed class AuthController(IMediator mediator, ILogger<AuthController> logger) : ControllerBase
{
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiEnvelope<LoginResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Login([FromBody] LoginCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Authentication attempt for {Email}", request.Email);
        var result = await mediator.Send(request, cancellationToken);
        return Ok(new ApiEnvelope<LoginResponse>(true, 200, "Authenticated.", result));
    }

    [HttpPost("refresh")]
    [ProducesResponseType(typeof(ApiEnvelope<LoginResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Ok(new ApiEnvelope<LoginResponse>(true, 200, "Token refreshed.", result));
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new ApiEnvelope<object>(true, 200, "Current user", new
        {
            User.Identity?.Name,
            Role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value
        }));
    }
}
