using CMS.IdentityService.Application.Interfaces;
using CMS.IdentityService.Domain.Entities;
using CMS.IdentityService.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CMS.IdentityService.API.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtService _jwtService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtService jwtService,
        IRefreshTokenRepository refreshTokenRepository,
        ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
        _refreshTokenRepository = refreshTokenRepository;
        _logger = logger;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            if (!user.IsActive)
            {
                return StatusCode(403, new { message = "Account is disabled. Contact support." });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var accessToken = _jwtService.GenerateAccessToken(user, roles);
                var refreshTokenString = _jwtService.GenerateRefreshToken();
                var tokenFamily = _jwtService.GetTokenFamily();

                var refreshToken = RefreshToken.Create(
                    user.Id,
                    refreshTokenString,
                    tokenFamily,
                    DateTime.UtcNow.AddDays(7));

                await _refreshTokenRepository.AddAsync(refreshToken);

                return Ok(new
                {
                    accessToken,
                    refreshToken = refreshTokenString,
                    expiresIn = 900, // 15 minutes in seconds
                    user = new
                    {
                        id = user.Id,
                        email = user.Email,
                        firstName = user.FirstName,
                        lastName = user.LastName,
                        role = roles.FirstOrDefault() ?? "Customer",
                        roles = roles
                    }
                });
            }

            if (result.IsLockedOut)
            {
                return StatusCode(403, new { message = "Account locked. Try again in 15 minutes." });
            }

            if (result.IsNotAllowed)
            {
                return StatusCode(403, new { message = "Login not allowed. Please confirm your email." });
            }

            return Unauthorized(new { message = "Invalid email or password." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user {Email}", dto.Email);
            return StatusCode(500, new { message = "Internal server error." });
        }
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        try
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                return Conflict(new { message = "Email already exists." });
            }

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Phone = dto.Phone,
                CompanyName = dto.CompanyName,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new { message = "Registration failed.", errors = result.Errors.Select(e => e.Description) });
            }

            await _userManager.AddToRoleAsync(user, "Customer");

            return StatusCode(201, new { message = "Registration successful. You can now log in." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for user {Email}", dto.Email);
            return StatusCode(500, new { message = "Internal server error." });
        }
    }
}

public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RegisterDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? CompanyName { get; set; }
}
