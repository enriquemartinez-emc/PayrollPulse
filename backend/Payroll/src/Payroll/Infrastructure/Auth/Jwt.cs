using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Payroll.Infrastructure.Auth;

public class JwtSettings
{
    public required string Key { get; init; }
    public required double ExpiryMinutes { get; init; }
}

public class Jwt(IOptions<JwtSettings> options)
{
    private readonly JwtSettings _settings = options.Value;
    private readonly JwtSecurityTokenHandler _handler = new();

    public string GenerateToken(User user)
    {
        var key = SecurityKey(_settings.Key);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("name", user.DisplayName ?? string.Empty),
            new Claim(ClaimTypes.Role, user.Role),
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(options.Value.ExpiryMinutes!),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        return _handler.WriteToken(token);
    }

    public static SymmetricSecurityKey SecurityKey(string key) => new(Encoding.ASCII.GetBytes(key));

    public ClaimsPrincipal? ValidateToken(string token)
    {
        var key = SecurityKey(_settings.Key);

        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero, // strict expiration
        };

        try
        {
            return _handler.ValidateToken(token, parameters, out _);
        }
        catch
        {
            return null; // invalid or expired
        }
    }
}
