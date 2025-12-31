using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ImpactHub.ERP.WebHost.Controllers;

[ApiController]
[Route("health")]
public class HealthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public HealthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet("db")]
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
            return StatusCode(500, ex.Message);
        }
    }
}
