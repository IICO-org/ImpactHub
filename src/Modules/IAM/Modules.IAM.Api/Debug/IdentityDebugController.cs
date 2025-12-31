using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Modules.IAM.Application.Queries.ResolveUserId;

namespace Modules.IAM.Api.Debug;

[ApiController]
[Route("api/iam/debug/identity")]
public sealed class IdentityDebugController : ControllerBase
{
    private readonly ResolveUserIdQueryHandler _handler;
    private readonly IHostEnvironment _env;

    public IdentityDebugController(ResolveUserIdQueryHandler handler, IHostEnvironment env)
    {
        _handler = handler;
        _env = env;
    }

    [AllowAnonymous]
    [HttpGet("resolve")]
    public async Task<IActionResult> Resolve(
        [FromQuery] string provider,
        [FromQuery] string subjectId,
        [FromQuery] string? issuer = null,
        CancellationToken cancellationToken = default)
    {
        // Debug endpoints only work in Development
        if (!_env.IsDevelopment())
            return NotFound();

        var query = new ResolveUserIdQuery(provider, issuer, subjectId);
        var userId = await _handler.HandleAsync(query, cancellationToken);

        if (userId is null)
            return NotFound("No user mapped to this external identity.");

        return Ok(new { UserId = userId });
    }
}
