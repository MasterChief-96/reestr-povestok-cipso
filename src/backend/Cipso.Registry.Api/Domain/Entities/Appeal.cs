using System.ComponentModel.DataAnnotations;

namespace Cipso.Registry.Api.Domain;

public sealed class Appeal
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SummonsId { get; set; }
    public Summons Summons { get; set; } = null!;
    [MaxLength(64)] public string Type { get; set; } = string.Empty;
    [MaxLength(2000)] public string Text { get; set; } = string.Empty;
    public AppealStatus Status { get; set; } = AppealStatus.Submitted;
    public DateTimeOffset SubmittedAt { get; set; } = DateTimeOffset.UtcNow;
}
