using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CMS.WarehouseService.Infrastructure.Services;

/// <summary>
/// Attaches a system-level JWT Bearer token to all outgoing HTTP requests
/// so service-to-service calls pass the [Authorize] gate on target APIs.
/// </summary>
public class ServiceAuthDelegatingHandler : DelegatingHandler
{
    private readonly IConfiguration _configuration;
    private string? _cachedToken;
    private DateTime _tokenExpiry = DateTime.MinValue;

    public ServiceAuthDelegatingHandler(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Generate or reuse a system JWT
        if (_cachedToken is null || DateTime.UtcNow >= _tokenExpiry)
        {
            _cachedToken = GenerateSystemToken();
            _tokenExpiry = DateTime.UtcNow.AddMinutes(55); // Token valid 60 min, refresh at 55
        }

        request.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _cachedToken);

        return await base.SendAsync(request, cancellationToken);
    }

    private string GenerateSystemToken()
    {
        var secret = _configuration["Jwt:Secret"]
            ?? "CMS-Super-Secret-Key-That-Is-At-Least-32-Characters-Long!";
        var issuer = _configuration["Jwt:Issuer"] ?? "CMS.IdentityService";
        var audience = _configuration["Jwt:Audience"] ?? "CMS.Clients";

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "system-warehouse-service"),
            new Claim(ClaimTypes.Name, "WarehouseService"),
            new Claim(ClaimTypes.Role, "SuperAdmin") // System services get full access
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
