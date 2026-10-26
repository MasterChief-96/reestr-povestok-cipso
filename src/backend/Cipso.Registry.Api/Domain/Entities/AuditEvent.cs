using System.ComponentModel.DataAnnotations;

namespace Cipso.Registry.Api.Domain;

public sealed class AuditEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SummonsId { get; set; }
    public Summons Summons { get; set; } = null!;
    [MaxLength(100)] public string Action { get; set; } = string.Empty;
    [MaxLength(120)] public string Actor { get; set; } = string.Empty;
    public DateTimeOffset OccurredAt { get; set; } = DateTimeOffset.UtcNow;
    [MaxLength(2000)] public string? Details { get; set; }
}
