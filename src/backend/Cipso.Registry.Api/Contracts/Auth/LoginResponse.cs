using System.ComponentModel.DataAnnotations;

namespace Cipso.Registry.Api.Contracts;

public sealed record LoginResponse(
    string Token,
    Guid AccountId,
    string DisplayName,
    string Role,
    string Provider,
    DateTimeOffset ExpiresAt);
