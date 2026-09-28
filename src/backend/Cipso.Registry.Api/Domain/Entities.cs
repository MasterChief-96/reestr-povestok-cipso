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

public sealed class AuthorityOffice
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [MaxLength(32)] public string Code { get; set; } = string.Empty;
    [MaxLength(180)] public string Name { get; set; } = string.Empty;
    [MaxLength(120)] public string Region { get; set; } = string.Empty;
    public List<Employee> Employees { get; set; } = [];
    public List<Summons> Summonses { get; set; } = [];
}

public sealed class Employee
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AuthorityOfficeId { get; set; }
    public AuthorityOffice AuthorityOffice { get; set; } = null!;
    [MaxLength(32)] public string PersonnelNumber { get; set; } = string.Empty;
    [MaxLength(180)] public string FullName { get; set; } = string.Empty;
    [MaxLength(64)] public string Role { get; set; } = "Operator";
    public List<Summons> CreatedSummonses { get; set; } = [];
}

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
