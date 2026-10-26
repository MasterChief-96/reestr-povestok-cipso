using System.ComponentModel.DataAnnotations;

namespace Cipso.Registry.Api.Domain;

public sealed class Summons
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [MaxLength(64)] public string Number { get; set; } = string.Empty;
    public Guid CitizenId { get; set; }
    public Citizen Citizen { get; set; } = null!;
    public Guid AuthorityOfficeId { get; set; }
    public AuthorityOffice AuthorityOffice { get; set; } = null!;
    public Guid CreatedByEmployeeId { get; set; }
    public Employee CreatedByEmployee { get; set; } = null!;
    public DateOnly IssuedAt { get; set; }
    public DateTimeOffset DueAt { get; set; }
    [MaxLength(500)] public string Reason { get; set; } = string.Empty;
    public SummonsStatus Status { get; set; } = SummonsStatus.Draft;
    [MaxLength(1000)] public string? Comment { get; set; }
    public List<SummonsStatusHistory> StatusHistory { get; set; } = [];
    public List<Notification> Notifications { get; set; } = [];
    public List<Appeal> Appeals { get; set; } = [];
    public List<Document> Documents { get; set; } = [];
    public List<AuditEvent> AuditEvents { get; set; } = [];
}
