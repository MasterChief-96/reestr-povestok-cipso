using System.ComponentModel.DataAnnotations;

namespace Cipso.Registry.Api.Domain;

public sealed class Citizen
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [MaxLength(64)] public string RegistryNumber { get; set; } = string.Empty;
    [MaxLength(100)] public string LastName { get; set; } = string.Empty;
    [MaxLength(100)] public string FirstName { get; set; } = string.Empty;
    [MaxLength(100)] public string? MiddleName { get; set; }
    public DateOnly BirthDate { get; set; }
    [MaxLength(255)] public string? Email { get; set; }
    [MaxLength(32)] public string? Phone { get; set; }
    public bool IsWrittenOff { get; set; }
    public DateTimeOffset? WrittenOffAt { get; set; }
    [MaxLength(180)] public string? WrittenOffBy { get; set; }
    public Address? Address { get; set; }
    public List<Summons> Summonses { get; set; } = [];
}
