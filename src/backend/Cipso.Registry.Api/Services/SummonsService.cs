using Cipso.Registry.Api.Contracts;
using Cipso.Registry.Api.Data;
using Cipso.Registry.Api.Domain;
using Microsoft.EntityFrameworkCore;
using Cipso.Registry.Api.Validation;

namespace Cipso.Registry.Api.Services;

public sealed class SummonsService(AppDbContext db)
{
    public async Task<Summons> CreateAsync(CreateSummonsRequest request)
    {
        if (request.DueAt.Date < request.IssuedAt.ToDateTime(TimeOnly.MinValue).Date)
            throw new FieldValidationException("dueAt", "DueAt: срок явки не может быть раньше даты формирования.");

        if (await db.Summonses.AnyAsync(x => x.Number == request.Number))
            throw new FieldValidationException("number", "Повестка с таким номером уже существует.", 409);

        var citizen = await db.Citizens.FindAsync(request.CitizenId)
            ?? throw new FieldValidationException("citizenId", "Выбранный призывник не найден.", 404);

        if (citizen.IsWrittenOff)
            throw new FieldValidationException("citizenId", "Нельзя выписать повестку списанному призывнику.", 409);

        var office = await db.AuthorityOffices.FindAsync(request.AuthorityOfficeId)
            ?? throw new FieldValidationException("authorityOfficeId", "Выбранный военкомат не найден.", 404);
        var employee = await db.Employees.FindAsync(request.CreatedByEmployeeId)
            ?? throw new FieldValidationException("authorityOfficeId", "У выбранного военкомата нет секретаря.", 404);

        if (employee.AuthorityOfficeId != office.Id)
            throw new FieldValidationException("authorityOfficeId", "Секретарь не относится к выбранному военкомату.");

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
