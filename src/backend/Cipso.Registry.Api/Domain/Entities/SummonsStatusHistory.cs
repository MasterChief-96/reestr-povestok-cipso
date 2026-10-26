using System.ComponentModel.DataAnnotations;

namespace Cipso.Registry.Api.Domain;

public sealed class SummonsStatusHistory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SummonsId { get; set; }
    public Summons Summons { get; set; } = null!;
    public SummonsStatus? FromStatus { get; set; }
    public SummonsStatus ToStatus { get; set; }
    public DateTimeOffset ChangedAt { get; set; } = DateTimeOffset.UtcNow;
    [MaxLength(120)] public string ChangedBy { get; set; } = string.Empty;
    [MaxLength(500)] public string? Comment { get; set; }
}
