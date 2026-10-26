using System.ComponentModel.DataAnnotations;

namespace Cipso.Registry.Api.Domain;

public sealed class SystemAccount
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [MaxLength(96)] public string ExternalSubject { get; set; } = string.Empty;
    [MaxLength(180)] public string DisplayName { get; set; } = string.Empty;
    [MaxLength(64)] public string Role { get; set; } = "Observer";
    [MaxLength(64)] public string? CitizenRegistryNumber { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
