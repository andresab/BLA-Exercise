using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookCatalog.Application.Users;
using BookCatalog.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BookCatalog.Infrastructure.Auth;

public sealed class JwtTokenService : IJwtTokenService
{
    private const string KeyName = "Jwt:Key";
    private const string IssuerName = "Jwt:Issuer";
    private const string AudienceName = "Jwt:Audience";
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(User user)
    {
        var key = _configuration[KeyName] ?? throw new InvalidOperationException("JWT key is not configured.");
        var issuer = _configuration[IssuerName] ?? throw new InvalidOperationException("JWT issuer is not configured.");
        var audience = _configuration[AudienceName] ?? throw new InvalidOperationException("JWT audience is not configured.");
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("username", user.Username)
        };

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
