using CMS.IdentityService.Application.Commands.RegisterUser;
using CMS.IdentityService.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CMS.IdentityService.API.Controllers;

/// <summary>
/// One-time bootstrap endpoint to create the initial SuperAdmin.
/// Disable or remove this controller after first use in production.
/// </summary>
[ApiController]
[Route("api/v1/seed")]
[AllowAnonymous]
public class SeedController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IConfiguration _configuration;

    public SeedController(IMediator mediator, IConfiguration configuration)
    {
        _mediator = mediator;
        _configuration = configuration;
    }

    /// <summary>
    /// Creates the initial SuperAdmin user. Only works when no users exist yet.
    /// Protect this with a seed key in production.
    /// </summary>
    [HttpPost("superadmin")]
    public async Task<IActionResult> SeedSuperAdmin([FromBody] SeedRequest request)
    {
        // Simple protection — require a seed key from config
        var seedKey = _configuration["Seed:Key"];
        if (!string.IsNullOrWhiteSpace(seedKey) && request.SeedKey != seedKey)
            return Unauthorized(new { message = "Invalid seed key." });

        var registerRequest = new RegisterUserRequest
        {
            Email = request.Email,
            Password = request.Password,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Role = "SuperAdmin"
        };

        var result = await _mediator.Send(
            new RegisterUserCommand(registerRequest, "seed", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "localhost"));

        return result.Success
            ? StatusCode(StatusCodes.Status201Created, result)
            : BadRequest(result);
    }
}

public class SeedRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string SeedKey { get; set; } = string.Empty;
}
