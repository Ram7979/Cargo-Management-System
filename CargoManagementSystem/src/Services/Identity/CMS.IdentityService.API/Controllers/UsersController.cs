using System.Security.Claims;
using CMS.IdentityService.Application.Commands.RegisterUser;
using CMS.IdentityService.Application.Commands.UpdateUserRoles;
using CMS.IdentityService.Application.DTOs;
using CMS.IdentityService.Application.Queries.GetAuditLogs;
using CMS.IdentityService.Application.Queries.GetUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CMS.IdentityService.API.Controllers;

[ApiController]
[Route("api/v1/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,OpsManager")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        var actorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var result = await _mediator.Send(new RegisterUserCommand(request, actorId, ipAddress));
        return result.Success ? StatusCode(StatusCodes.Status201Created, result) : BadRequest(result);
    }

    [HttpGet("{userId:guid}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> GetUser(Guid userId)
    {
        var result = await _mediator.Send(new GetUserQuery(userId));
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPut("{userId:guid}/roles")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> UpdateRoles(Guid userId, [FromBody] UpdateUserRolesRequest request)
    {
        var actorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var result = await _mediator.Send(new UpdateUserRolesCommand(userId, request, actorId, ipAddress));
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{userId:guid}/audit-logs")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> GetAuditLogs(Guid userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new GetAuditLogsQuery(userId.ToString(), page, pageSize));
        return Ok(result);
    }
}
