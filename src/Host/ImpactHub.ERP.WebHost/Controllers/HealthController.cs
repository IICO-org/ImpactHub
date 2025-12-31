using ImpactHub.ERP.WebHost.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ImpactHub.ERP.WebHost.Controllers;

/// <summary>
/// Health and diagnostic endpoints for the WebHost.
///
/// IMPORTANT:
/// ----------
/// • These endpoints are NOT business APIs.
/// • They exist to validate infrastructure, wiring, and security behavior.
/// • Some endpoints are intended for Development/Operations only
///   and should be removed or restricted in production.
///
/// This controller deliberately stays simple and explicit.
/// </summary>
[ApiController]
[Route("health")]
public class HealthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// IConfiguration is injected to allow access to
    /// environment-specific settings such as connection strings.
    ///
    /// This avoids hardcoding any infrastructure detail.
    /// </summary>
    public HealthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Database connectivity check.
    ///
    /// What this endpoint proves:
    /// --------------------------
    /// • Connection string is correctly loaded from configuration
    /// • SQL Server is reachable from the WebHost
    /// • Basic ADO.NET connectivity works
    ///
    /// What it does NOT prove:
    /// -----------------------
    /// • Data correctness
    /// • IAM authorization
    /// • Business rules
    ///
    /// This is a pure infrastructure health probe.
    /// </summary>
    [HttpGet("db")]
    [AllowAnonymous]
    public async Task<IActionResult> CheckDatabase()
    {
        var connectionString = _configuration.GetConnectionString("ImpactHubDb");

        try
        {
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            return Ok("Database connection OK");
        }
        catch (Exception ex)
        {
            // Returning 500 here is intentional:
            // this indicates an infrastructure failure, not a client error.
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// Authorization test endpoint (C9 validation).
    ///
    /// This endpoint exists ONLY to validate permission enforcement.
    ///
    /// Expected behavior:
    /// ------------------
    /// • 200 OK  → current user HAS the USERS.VIEW permission
    /// • 403     → user is authenticated but lacks the permission
    /// • 401     → user identity was not resolved (authentication failure)
    ///
    /// Why this endpoint exists:
    /// -------------------------
    /// • Proves that [RequiresPermission] attribute works
    /// • Proves that CurrentAccess + AccessProfileProvider are wired correctly
    /// • Proves that IAM data drives runtime authorization
    ///
    /// WARNING:
    /// --------
    /// • This endpoint is for DEVELOPMENT / VALIDATION only
    /// • It should be removed or locked down in production
    /// </summary>
    [HttpGet("auth/users-view")]
    [RequiresPermission("USERS.VIEW")]
    public IActionResult TestUsersViewPermission()
    {
        return Ok(new
        {
            message = "USERS.VIEW permission granted",
            testedAt = DateTime.UtcNow
        });
    }
}
