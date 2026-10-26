using System.ComponentModel.DataAnnotations;

namespace Cipso.Registry.Api.Domain;

public sealed class DeliveryAttempt
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid NotificationId { get; set; }
    public Notification Notification { get; set; } = null!;
    public int AttemptNumber { get; set; }
    [MaxLength(64)] public string Result { get; set; } = string.Empty;
    public DateTimeOffset AttemptedAt { get; set; } = DateTimeOffset.UtcNow;
    [MaxLength(500)] public string? ProviderMessage { get; set; }
}
