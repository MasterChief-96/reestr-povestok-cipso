using Cipso.Registry.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Cipso.Registry.Api.Data;

public static class SeedData
{
    private static readonly (string LastName, string FirstName, string MiddleName)[] DemoNames =
    [
        ("Сидоров", "Сидор", "Сидорович"),
        ("Петров", "Петр", "Петрович"),
        ("Иванов", "Иван", "Иванович"),
        ("Смирнов", "Алексей", "Сергеевич"),
        ("Кузнецов", "Дмитрий", "Андреевич"),
        ("Попов", "Максим", "Олегович"),
        ("Соколов", "Артем", "Игоревич"),
        ("Лебедев", "Никита", "Романович"),
        ("Козлов", "Михаил", "Викторович"),
        ("Новиков", "Егор", "Алексеевич"),
        ("Морозов", "Кирилл", "Денисович"),
        ("Волков", "Роман", "Павлович")
    ];

    private static readonly SummonsStatus[] DemoStatuses =
    [
        SummonsStatus.Issued,
        SummonsStatus.Delivered,
        SummonsStatus.Acknowledged,
        SummonsStatus.Completed,
        SummonsStatus.Cancelled
    ];

    public static async Task InitializeAsync(AppDbContext db)
    {
        var offices = await EnsureOfficesAsync(db);
        var citizens = await EnsureCitizensAsync(db);

        await EnsureSummonsesAsync(db, offices, citizens);
        await db.SaveChangesAsync();
    }

    private static async Task<List<AuthorityOffice>> EnsureOfficesAsync(AppDbContext db)
    {
        var offices = await db.AuthorityOffices
            .Include(x => x.Employees)
            .OrderBy(x => x.Code)
            .ToListAsync();

        var office1 = offices.FirstOrDefault(x => x.Code == "TEST-01");
        if (office1 is null)
        {
            office1 = new AuthorityOffice
            {
                Code = "TEST-01",
                Name = "Учебное подразделение №1",
                Region = "Тестовый регион"
            };
            db.AuthorityOffices.Add(office1);
            offices.Add(office1);
        }

        if (office1.Employees.Count == 0)
        {
            office1.Employees.Add(new Employee
            {
                PersonnelNumber = "EMP-0001",
                FullName = "Иванов Иван Оператор",
                Role = "Operator"
            });
        }

        var office2 = offices.FirstOrDefault(x => x.Code == "TEST-02");
        if (office2 is null)
        {
            office2 = new AuthorityOffice
            {
                Code = "TEST-02",
                Name = "Учебное подразделение №2",
                Region = "Тестовый регион"
            };
            office2.Employees.Add(new Employee
            {
                PersonnelNumber = "EMP-0002",
                FullName = "Петров Петр Оператор",
                Role = "Operator"
            });
            db.AuthorityOffices.Add(office2);
            offices.Add(office2);
        }
        else if (office2.Employees.Count == 0)
        {
            office2.Employees.Add(new Employee
            {
                PersonnelNumber = "EMP-0002",
                FullName = "Петров Петр Оператор",
                Role = "Operator"
            });
        }

        await db.SaveChangesAsync();

        return offices
            .Where(x => x.Code is "TEST-01" or "TEST-02")
            .OrderBy(x => x.Code)
            .ToList();
    }

    private static async Task<List<Citizen>> EnsureCitizensAsync(AppDbContext db)
    {
        var existing = await db.Citizens
            .Include(x => x.Address)
            .Where(x => x.RegistryNumber.StartsWith("TEST-"))
            .ToListAsync();

        for (var i = 1; i <= DemoNames.Length; i++)
        {
            var registryNumber = $"TEST-{i:0000}";
            if (existing.Any(x => x.RegistryNumber == registryNumber))
                continue;

            var name = DemoNames[i - 1];
            var citizen = new Citizen
            {
                RegistryNumber = registryNumber,
                LastName = name.LastName,
                FirstName = name.FirstName,
                MiddleName = name.MiddleName,
                BirthDate = new DateOnly(1988 + i, ((i - 1) % 12) + 1, Math.Min(20, i + 2)),
                Email = $"demo{i:00}@example.test",
                Phone = $"+7000000{i:0000}",
                Address = new Address
                {
                    PostalCode = $"{100000 + i}",
                    Region = "Тестовый регион",
                    City = i % 2 == 0 ? "Демо-Сити" : "Тестоград",
                    Street = i % 3 == 0 ? "Проектная" : "Учебная",
                    Building = $"{i}",
                    Apartment = $"{10 + i}"
                }
            };

            db.Citizens.Add(citizen);
            existing.Add(citizen);
        }

        await db.SaveChangesAsync();

        return existing
            .OrderBy(x => x.RegistryNumber)
            .Take(DemoNames.Length)
            .ToList();
    }

    private static async Task EnsureSummonsesAsync(
        AppDbContext db,
        IReadOnlyList<AuthorityOffice> offices,
        IReadOnlyList<Citizen> citizens)
    {
        var existingNumberList = await db.Summonses
            .Where(x => x.Number.StartsWith("CIPSO-2026-"))
            .Select(x => x.Number)
            .ToListAsync();
        var existingNumbers = existingNumberList.ToHashSet(StringComparer.Ordinal);

        var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);

        for (var i = 1; i <= 25; i++)
        {
            var number = $"CIPSO-2026-{i:0000}";
            if (existingNumbers.Contains(number))
                continue;

            var citizen = citizens[(i - 1) % citizens.Count];
            var office = offices[(i - 1) % offices.Count];
            var employee = office.Employees.First();
            var issuedAt = today.AddDays(-i);
            var status = DemoStatuses[(i - 1) % DemoStatuses.Length];

            var summons = new Summons
            {
                Number = number,
                Citizen = citizen,
                AuthorityOffice = office,
                CreatedByEmployee = employee,
                IssuedAt = issuedAt,
                DueAt = new DateTimeOffset(
                    issuedAt.Year,
                    issuedAt.Month,
                    issuedAt.Day,
                    10 + (i % 6),
                    0,
                    0,
                    TimeSpan.Zero).AddDays(7),
                Reason = $"Учебное демонстрационное оповещение №{i}",
                Status = status,
                Comment = "Синтетические данные для демонстрации пагинации и разделов интерфейса"
            };

            summons.StatusHistory.Add(new SummonsStatusHistory
            {
                FromStatus = SummonsStatus.Draft,
                ToStatus = SummonsStatus.Issued,
                ChangedAt = DateTimeOffset.UtcNow.AddDays(-i),
                ChangedBy = employee.PersonnelNumber,
                Comment = "Создано seed-данными"
            });

            if (status != SummonsStatus.Issued)
            {
                summons.StatusHistory.Add(new SummonsStatusHistory
                {
                    FromStatus = SummonsStatus.Issued,
                    ToStatus = status,
                    ChangedAt = DateTimeOffset.UtcNow.AddDays(-(i / 2.0)),
                    ChangedBy = employee.PersonnelNumber,
                    Comment = "Демонстрационное изменение статуса"
                });
            }

            summons.AuditEvents.Add(new AuditEvent
            {
                Action = "SummonsCreated",
                Actor = "seed",
                OccurredAt = DateTimeOffset.UtcNow.AddDays(-i),
                Details = $"Synthetic demo record #{i}"
            });

            if (i % 2 == 0)
            {
                var notification = new Notification
                {
                    Channel = i % 4 == 0 ? NotificationChannel.Sms : NotificationChannel.Email,
                    DestinationMasked = i % 4 == 0 ? "***1234" : "de***@example.test",
                    Status = NotificationStatus.Delivered,
                    CreatedAt = DateTimeOffset.UtcNow.AddDays(-i).AddHours(2)
                };
                notification.DeliveryAttempts.Add(new DeliveryAttempt
                {
                    AttemptNumber = 1,
                    Result = "Delivered",
                    AttemptedAt = notification.CreatedAt.AddMinutes(1),
                    ProviderMessage = "Mock provider: delivery simulated successfully"
                });
                summons.Notifications.Add(notification);
            }

            if (i % 4 == 0)
            {
                summons.Appeals.Add(new Appeal
                {
                    Type = i % 8 == 0 ? "Clarification" : "Schedule",
                    Text = $"Учебное обращение по записи {number}",
                    Status = i % 8 == 0 ? AppealStatus.InReview : AppealStatus.Submitted,
                    SubmittedAt = DateTimeOffset.UtcNow.AddDays(-Math.Max(1, i - 2))
                });
            }

            if (i % 3 == 0)
            {
                summons.Documents.Add(new Document
                {
                    FileName = $"demo-document-{i:00}.pdf",
                    MimeType = "application/pdf",
                    StorageUri = $"demo://documents/demo-document-{i:00}.pdf",
                    CreatedAt = DateTimeOffset.UtcNow.AddDays(-Math.Max(1, i - 1))
                });
            }

            db.Summonses.Add(summons);
        }

        await db.SaveChangesAsync();
    }
}
