namespace Cipso.Registry.Api.Auth;

public sealed class JwtOptions
{
    public string Issuer { get; init; } = "cipso-registry";
    public string Audience { get; init; } = "cipso-registry-client";
    public string Key { get; init; } = string.Empty;
    public int ExpiresMinutes { get; init; } = 120;
}

public sealed class DemoUser
{
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string Role { get; init; } = "Observer";
}
