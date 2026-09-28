using System.ComponentModel.DataAnnotations;

namespace Cipso.Registry.Api.Contracts;

public sealed record LoginRequest(
    [Required] string Username,
    [Required] string Password);

public sealed record LoginResponse(
    string Token,
    string Username,
    string DisplayName,
    string Role,
    DateTimeOffset ExpiresAt);
