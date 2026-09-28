using Cipso.Registry.Api.Contracts;
using Cipso.Registry.Api.Data;
using Cipso.Registry.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Cipso.Registry.Api.Services;

public sealed class SummonsService(AppDbContext db)
{
    public async Task<Summons> CreateAsync(CreateSummonsRequest request)
    {
        if (request.DueAt.Date < request.IssuedAt.ToDateTime(TimeOnly.MinValue).Date)
            throw new ArgumentException("DueAt cannot be earlier than IssuedAt.");

        if (await db.Summonses.AnyAsync(x => x.Number == request.Number))
            throw new InvalidOperationException("Summons number already exists.");

        var citizen = await db.Citizens.FindAsync(request.CitizenId)
            ?? throw new KeyNotFoundException("Citizen not found.");
        var office = await db.AuthorityOffices.FindAsync(request.AuthorityOfficeId)
            ?? throw new KeyNotFoundException("Authority office not found.");
        var employee = await db.Employees.FindAsync(request.CreatedByEmployeeId)
            ?? throw new KeyNotFoundException("Employee not found.");

        var entity = new Summons
        {
            Number = request.Number.Trim(),
            Citizen = citizen,
            AuthorityOffice = office,
            CreatedByEmployee = employee,
            IssuedAt = request.IssuedAt,
            DueAt = request.DueAt,
            Reason = request.Reason.Trim(),
            Comment = request.Comment,
            Status = SummonsStatus.Issued
        };
        entity.StatusHistory.Add(new SummonsStatusHistory
        {
            FromStatus = SummonsStatus.Draft,
            ToStatus = SummonsStatus.Issued,
            ChangedBy = employee.PersonnelNumber,
            Comment = "Created"
        });
        entity.AuditEvents.Add(new AuditEvent
        {
            Action = "SummonsCreated",
            Actor = employee.PersonnelNumber,
            Details = $"Number={request.Number}"
        });

        db.Summonses.Add(entity);
        await db.SaveChangesAsync();
        return entity;
    }

    public async Task<Summons> ChangeStatusAsync(Guid id, ChangeStatusRequest request)
    {
        var entity = await db.Summonses
            .Include(x => x.StatusHistory)
            .Include(x => x.AuditEvents)
            .FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new KeyNotFoundException("Summons not found.");

        if (entity.Status == request.Status)
            throw new InvalidOperationException("Summons already has this status.");

        var previous = entity.Status;
        entity.Status = request.Status;
        entity.StatusHistory.Add(new SummonsStatusHistory
        {
            FromStatus = previous,
            ToStatus = request.Status,
            ChangedBy = request.Actor.Trim(),
            Comment = request.Comment
        });
        entity.AuditEvents.Add(new AuditEvent
        {
            Action = "StatusChanged",
            Actor = request.Actor.Trim(),
            Details = $"{previous} -> {request.Status}"
        });
        await db.SaveChangesAsync();
        return entity;
    }

    public async Task<Notification> SendMockNotificationAsync(Guid id, CreateNotificationRequest request)
    {
        var summons = await db.Summonses.FindAsync(id)
            ?? throw new KeyNotFoundException("Summons not found.");

        var masked = MaskDestination(request.Destination);
        var notification = new Notification
        {
            Summons = summons,
            Channel = request.Channel,
            DestinationMasked = masked,
            Status = NotificationStatus.Delivered
        };
        notification.DeliveryAttempts.Add(new DeliveryAttempt
        {
            AttemptNumber = 1,
            Result = "Delivered",
            ProviderMessage = "Mock provider: delivery simulated successfully"
        });
        db.Notifications.Add(notification);
        db.AuditEvents.Add(new AuditEvent
        {
            Summons = summons,
            Action = "NotificationSent",
            Actor = "demo.operator",
            Details = $"Channel={request.Channel}; Destination={masked}"
        });
        await db.SaveChangesAsync();
        return notification;
    }

    private static string MaskDestination(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return "***";
        if (value.Contains('@'))
        {
            var parts = value.Split('@', 2);
            return parts[0][..Math.Min(2, parts[0].Length)] + "***@" + parts[1];
        }
        return value.Length <= 4 ? "***" : $"***{value[^4..]}";
    }
}
