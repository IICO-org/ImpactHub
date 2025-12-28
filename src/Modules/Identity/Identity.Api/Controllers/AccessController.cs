

using Microsoft.AspNetCore.Authorization;
using Identity.Application.Commands.AssignRole;
using Identity.Application.Commands.RevokeAccess;
using Identity.Application.Queries.GetPermissions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/identity/access")]
[Authorize(AuthenticationSchemes = "Bearer")]

public class AccessController : ControllerBase
{
    private readonly IMediator _mediator;

    public AccessController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleCommand command)
    {
        var result = await _mediator.Send(command);

        if (result > 0)
            return Ok(new { Message = "تم إسناد الدور بنجاح", AssignmentId = result });

        return BadRequest("فشلت العملية، تأكد من صحة البيانات");
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<List<UserPermissionDto>>> GetUserPermissions(int userId)
    {
        var query = new GetUserPermissionsQuery(userId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPut("{id}/revoke")]
    public async Task<IActionResult> RevokeAccess(int id, [FromBody] int revokedBy)
    {
        var command = new RevokeAccessCommand(id, revokedBy);
        var result = await _mediator.Send(command);

        if (result)
            return Ok("تم إلغاء الصلاحية بنجاح");

        return NotFound("لم يتم العثور على هذه الصلاحية");
    }
}
