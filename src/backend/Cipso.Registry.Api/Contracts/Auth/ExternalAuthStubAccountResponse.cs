using System.ComponentModel.DataAnnotations;

namespace Cipso.Registry.Api.Contracts;

public sealed record ExternalAuthStubAccountResponse(
    Guid Id,
    string DisplayName,
    string Role,
    string? CitizenRegistryNumber);
