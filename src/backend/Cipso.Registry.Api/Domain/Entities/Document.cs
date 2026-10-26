using System.ComponentModel.DataAnnotations;

namespace Cipso.Registry.Api.Domain;

public sealed class Document
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SummonsId { get; set; }
    public Summons Summons { get; set; } = null!;
    [MaxLength(255)] public string FileName { get; set; } = string.Empty;
    [MaxLength(120)] public string MimeType { get; set; } = string.Empty;
    [MaxLength(1000)] public string StorageUri { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
