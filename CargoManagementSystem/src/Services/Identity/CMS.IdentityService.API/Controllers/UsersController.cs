using System.Security.Claims;
using CMS.IdentityService.Application.Commands.DeactivateUser;
using CMS.IdentityService.Application.Commands.RegisterUser;
using CMS.IdentityService.Application.Commands.UpdateUser;
using CMS.IdentityService.Application.Commands.UpdateUserRoles;
using CMS.IdentityService.Application.DTOs;
using CMS.IdentityService.Application.Queries.GetAuditLogs;
using CMS.IdentityService.Application.Queries.GetUser;
using CMS.IdentityService.Application.Queries.GetUsers;
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

    private string ActorId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";
    private string IpAddress => HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

    /// <summary>Register a new staff user account.</summary>
    [HttpPost]
    [Authorize(Roles = "SuperAdmin,OpsManager")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        var result = await _mediator.Send(new RegisterUserCommand(request, ActorId, IpAddress));
        return result.Success ? StatusCode(StatusCodes.Status201Created, result) : BadRequest(result);
    }

    /// <summary>Get paginated list of all users with optional filters.</summary>
    [HttpGet]
    [Authorize(Roles = "SuperAdmin,OpsManager")]
    public async Task<IActionResult> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? role = null,
        [FromQuery] bool? isActive = null)
    {
        var result = await _mediator.Send(new GetUsersQuery(page, pageSize, search, role, isActive));
        return Ok(result);
    }

    /// <summary>Get a single user by ID.</summary>
    [HttpGet("{userId:guid}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> GetUser(Guid userId)
    {
        var result = await _mediator.Send(new GetUserQuery(userId));
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>Update a user's profile fields (FirstName, LastName).</summary>
    [HttpPatch("{userId:guid}")]
    [Authorize(Roles = "SuperAdmin,OpsManager")]
    public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UpdateUserRequest request)
    {
        var result = await _mediator.Send(new UpdateUserCommand(userId, request, ActorId, IpAddress));
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>Assign roles to a user (replaces all existing roles).</summary>
    [HttpPut("{userId:guid}/roles")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> UpdateRoles(Guid userId, [FromBody] UpdateUserRolesRequest request)
    {
        var result = await _mediator.Send(new UpdateUserRolesCommand(userId, request, ActorId, IpAddress));
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>Deactivate (soft-delete) a user account.</summary>
    [HttpDelete("{userId:guid}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> DeactivateUser(Guid userId)
    {
        var result = await _mediator.Send(new DeactivateUserCommand(userId, ActorId, IpAddress));
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>Get paginated audit log for a specific user.</summary>
    [HttpGet("{userId:guid}/audit-logs")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> GetAuditLogs(
        Guid userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new GetAuditLogsQuery(userId.ToString(), page, pageSize));
        return Ok(result);
    }
}
