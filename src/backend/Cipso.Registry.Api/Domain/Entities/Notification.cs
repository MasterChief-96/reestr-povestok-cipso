using System.ComponentModel.DataAnnotations;

namespace Cipso.Registry.Api.Domain;

public sealed class Notification
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SummonsId { get; set; }
    public Summons Summons { get; set; } = null!;
    public NotificationChannel Channel { get; set; }
    [MaxLength(255)] public string DestinationMasked { get; set; } = string.Empty;
    public NotificationStatus Status { get; set; } = NotificationStatus.Queued;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public List<DeliveryAttempt> DeliveryAttempts { get; set; } = [];
}
