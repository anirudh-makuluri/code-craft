using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using api.Entities;
using Microsoft.IdentityModel.Tokens;

namespace api.Services;

public class TokenService(IConfiguration configuration) : ITokenService
{
    public string CreateToken(User user)
    {
        var issuer = configuration["Jwt:Issuer"] ?? "codecraft";
        var audience = configuration["Jwt:Audience"] ?? "codecraft-ui";
        var key = configuration["Jwt:SigningKey"] ?? throw new InvalidOperationException("JWT signing key not configured");

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Username),
            new(ClaimTypes.Name, user.Username)
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddDays(30),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
