using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Cipso.Registry.Api.Domain;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Cipso.Registry.Api.Auth;

public sealed class JwtTokenService(IOptions<JwtOptions> jwtOptions)
{
    private readonly JwtOptions _jwt = jwtOptions.Value;

    public (string Token, DateTimeOffset ExpiresAt) CreateToken(SystemAccount account, string provider)
    {
        if (Encoding.UTF8.GetByteCount(_jwt.Key) < 32)
            throw new InvalidOperationException("JWT key must contain at least 32 UTF-8 bytes.");

        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(_jwt.ExpiresMinutes);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, account.ExternalSubject),
            new(ClaimTypes.NameIdentifier, account.Id.ToString()),
            new(ClaimTypes.Name, account.DisplayName),
            new(ClaimTypes.Role, account.Role),
            new("auth_provider", provider)
        };

        if (!string.IsNullOrWhiteSpace(account.CitizenRegistryNumber))
            claims.Add(new Claim("citizen_registry_number", account.CitizenRegistryNumber));

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: expiresAt.UtcDateTime,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
