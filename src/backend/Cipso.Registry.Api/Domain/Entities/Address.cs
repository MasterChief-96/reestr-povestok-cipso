using System.ComponentModel.DataAnnotations;

namespace Cipso.Registry.Api.Domain;

public sealed class Address
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CitizenId { get; set; }
    public Citizen Citizen { get; set; } = null!;
    [MaxLength(16)] public string PostalCode { get; set; } = string.Empty;
    [MaxLength(120)] public string Region { get; set; } = string.Empty;
    [MaxLength(120)] public string City { get; set; } = string.Empty;
    [MaxLength(120)] public string Street { get; set; } = string.Empty;
    [MaxLength(32)] public string Building { get; set; } = string.Empty;
    [MaxLength(32)] public string? Apartment { get; set; }
}
