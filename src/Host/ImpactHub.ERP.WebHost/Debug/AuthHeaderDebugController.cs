using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace ImpactHub.ERP.WebHost.Debug;

[ApiController]
[Route("api/debug/auth-header")]
public sealed class AuthHeaderDebugController : ControllerBase
{
    private readonly IHostEnvironment _env;

    public AuthHeaderDebugController(IHostEnvironment env)
    {
        _env = env;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Get()
    {
        if (!_env.IsDevelopment())
            return NotFound();

        var hasAuth = Request.Headers.ContainsKey("Authorization");
        var authValue = hasAuth ? Request.Headers["Authorization"].ToString() : "";

        return Ok(new
        {
            hasAuthorizationHeader = hasAuth,
            authorizationStartsWithBearer = authValue.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
        });
    }
}
