using System.ComponentModel.DataAnnotations;

namespace Cipso.Registry.Api.Contracts;

public sealed record ExternalAuthStubRequest(
    [Required] string Provider,
    Guid AccountId);

public sealed record ExternalAuthStubAccountResponse(
    Guid Id,
    string DisplayName,
    string Role,
    string? CitizenRegistryNumber);

public sealed record LoginResponse(
    string Token,
    Guid AccountId,
    string DisplayName,
    string Role,
    string Provider,
    DateTimeOffset ExpiresAt);

public sealed record CreateSystemAccountRequest(
    [Required] string DisplayName,
    [Required] string Role,
    CreateCitizenRequest? Citizen);
