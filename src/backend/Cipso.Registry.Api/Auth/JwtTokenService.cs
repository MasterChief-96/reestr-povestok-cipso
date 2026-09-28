using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Cipso.Registry.Api.Auth;

public sealed class JwtTokenService(IOptions<JwtOptions> jwtOptions, IConfiguration configuration)
{
    private readonly JwtOptions _jwt = jwtOptions.Value;
    private readonly IReadOnlyList<DemoUser> _users =
        configuration.GetSection("DemoAuth:Users").Get<List<DemoUser>>() ?? [];

    public DemoUser? ValidateCredentials(string username, string password) =>
        _users.FirstOrDefault(x =>
            string.Equals(x.Username, username, StringComparison.OrdinalIgnoreCase) &&
            x.Password == password);

    public (string Token, DateTimeOffset ExpiresAt) CreateToken(DemoUser user)
    {
        if (Encoding.UTF8.GetByteCount(_jwt.Key) < 32)
            throw new InvalidOperationException("JWT key must contain at least 32 UTF-8 bytes.");

        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(_jwt.ExpiresMinutes);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Username),
            new Claim(ClaimTypes.Name, user.DisplayName),
            new Claim(ClaimTypes.Role, user.Role)
        };

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
