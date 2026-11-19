using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Couchy.Api.Auth;
using Couchy.Api.Entities;
using Microsoft.IdentityModel.Tokens;

namespace Couchy.Api.Services;

public class JwtService
{
    private readonly JwtSettings _settings;

    public JwtService(JwtSettings settings)
    {
        _settings = settings;
    }

    /// <summary>Creates an HS256 token whose subject ("sub") is the username.</summary>
    public string GenerateToken(User user)
    {
        var now = DateTime.UtcNow;
        var key = new SymmetricSecurityKey(_settings.GetKeyBytes());

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(JwtRegisteredClaimNames.Iat, EpochTime.GetIntDate(now).ToString(), ClaimValueTypes.Integer64)
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: now.AddMinutes(_settings.ExpirationMinutes),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
