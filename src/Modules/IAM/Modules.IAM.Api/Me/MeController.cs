using Microsoft.AspNetCore.Mvc;
using Modules.IAM.Application.Queries.AccessProfile;

namespace Modules.IAM.Api.Me;

[ApiController]
[Route("api/me")]
public sealed class MeController : ControllerBase
{
    private readonly GetMyAccessProfileHandler _handler;

    public MeController(GetMyAccessProfileHandler handler)
    {
        _handler = handler;
    }

    [HttpGet("access-profile")]
    public Task<AccessProfileDto> GetAccessProfile(CancellationToken cancellationToken)
        => _handler.HandleAsync(new GetMyAccessProfileQuery(), cancellationToken);
}
