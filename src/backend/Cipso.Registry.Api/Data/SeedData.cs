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

    private static readonly string[] MilitaryReasons =
    [
        "Явка в военный комиссариат для уточнения документов воинского учёта",
        "Явка для сверки сведений воинского учёта",
        "Прохождение мероприятий призывного учёта",
        "Предоставление документов военно-учётного дела",
        "Уточнение персональных данных военно-учётной карточки"
    ];

    public static async Task InitializeAsync(AppDbContext db)
    {
        var offices = await EnsureOfficesAsync(db);
        var citizens = await EnsureCitizensAsync(db);

        await EnsureSystemAccountsAsync(db, citizens);
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
            office1 = new AuthorityOffice { Code = "TEST-01" };
            db.AuthorityOffices.Add(office1);
            offices.Add(office1);
        }

        office1.Name = "Учебный военный комиссариат района №1";
        office1.Region = "Тестовый регион";

        if (office1.Employees.Count == 0)
        {
            office1.Employees.Add(new Employee
            {
                PersonnelNumber = "EMP-0001",
                FullName = "Иванов Иван Иванович",
                Role = "Секретарь военно-учётного стола"
            });
        }
        else
        {
            office1.Employees[0].FullName = "Иванов Иван Иванович";
            office1.Employees[0].Role = "Секретарь военно-учётного стола";
        }

        var office2 = offices.FirstOrDefault(x => x.Code == "TEST-02");
        if (office2 is null)
        {
            office2 = new AuthorityOffice { Code = "TEST-02" };
            db.AuthorityOffices.Add(office2);
            offices.Add(office2);
        }

        office2.Name = "Учебный военный комиссариат района №2";
        office2.Region = "Тестовый регион";

        if (office2.Employees.Count == 0)
        {
            office2.Employees.Add(new Employee
            {
                PersonnelNumber = "EMP-0002",
                FullName = "Петров Петр Петрович",
                Role = "Секретарь военно-учётного стола"
            });
        }
        else
        {
            office2.Employees[0].FullName = "Петров Петр Петрович";
            office2.Employees[0].Role = "Секретарь военно-учётного стола";
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
                Email = $"conscript{i:00}@example.test",
                Phone = $"+7000000{i:0000}",
                Address = new Address
                {
                    PostalCode = $"{100000 + i}",
                    Region = "Тестовый регион",
                    City = i % 2 == 0 ? "Демо-Сити" : "Тестоград",
                    Street = i % 3 == 0 ? "Воинская" : "Учебная",
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

    private static async Task EnsureSystemAccountsAsync(
        AppDbContext db,
        IReadOnlyList<Citizen> citizens)
    {
        var accounts = await db.SystemAccounts.ToListAsync();

        var seeds = new[]
        {
            new SystemAccount
            {
                ExternalSubject = "demo-secretary",
                DisplayName = "Демо-секретарь",
                Role = "Operator"
            },
            new SystemAccount
            {
                ExternalSubject = "demo-commissar",
                DisplayName = "Демо-комиссар",
                Role = "Manager"
            },
            new SystemAccount
            {
                ExternalSubject = "demo-conscript",
                DisplayName = $"{citizens[0].LastName} {citizens[0].FirstName} {citizens[0].MiddleName}",
                Role = "Observer",
                CitizenRegistryNumber = citizens[0].RegistryNumber
            },
            new SystemAccount
            {
                ExternalSubject = "demo-automation-engineer",
                DisplayName = "Демо-инженер автоматизации",
                Role = "AutomationEngineer"
            }
        };

        foreach (var seed in seeds)
        {
            var existing = accounts.FirstOrDefault(x => x.ExternalSubject == seed.ExternalSubject);
            if (existing is null)
            {
                db.SystemAccounts.Add(seed);
                accounts.Add(seed);
                continue;
            }

            existing.DisplayName = seed.DisplayName;
            existing.Role = seed.Role;
            existing.CitizenRegistryNumber = seed.CitizenRegistryNumber;
            existing.IsActive = true;
        }

        await db.SaveChangesAsync();
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
                    9 + (i % 6),
                    0,
                    0,
                    TimeSpan.Zero).AddDays(7),
                Reason = MilitaryReasons[(i - 1) % MilitaryReasons.Length],
                Status = status,
                Comment = "Синтетическая запись учебного реестра повесток военного учёта"
            };

            summons.StatusHistory.Add(new SummonsStatusHistory
            {
                FromStatus = SummonsStatus.Draft,
                ToStatus = SummonsStatus.Issued,
                ChangedAt = DateTimeOffset.UtcNow.AddDays(-i),
                ChangedBy = employee.PersonnelNumber,
                Comment = "Повестка сформирована в учебном контуре"
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
                Details = "Synthetic military-accounting demo record"
            });

            if (i % 2 == 0)
            {
                var notification = new Notification
                {
                    Channel = i % 4 == 0 ? NotificationChannel.Sms : NotificationChannel.Email,
                    DestinationMasked = i % 4 == 0 ? "***1234" : "co***@example.test",
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
                    Type = i % 8 == 0 ? "Clarification" : "AttendanceDate",
                    Text = i % 8 == 0
                        ? $"Учебное обращение об уточнении сведений по повестке {number}"
                        : $"Учебное обращение об уточнении даты явки по повестке {number}",
                    Status = i % 8 == 0 ? AppealStatus.InReview : AppealStatus.Submitted,
                    SubmittedAt = DateTimeOffset.UtcNow.AddDays(-Math.Max(1, i - 2))
                });
            }

            if (i % 3 == 0)
            {
                summons.Documents.Add(new Document
                {
                    FileName = $"military-accounting-demo-{i:00}.pdf",
                    MimeType = "application/pdf",
                    StorageUri = $"demo://military-documents/military-accounting-demo-{i:00}.pdf",
                    CreatedAt = DateTimeOffset.UtcNow.AddDays(-Math.Max(1, i - 1))
                });
            }

            db.Summonses.Add(summons);
        }

        await db.SaveChangesAsync();

        var demoSummonses = await db.Summonses
            .Where(x => x.Number.StartsWith("CIPSO-2026-"))
            .ToListAsync();

        foreach (var summons in demoSummonses)
        {
            if (!int.TryParse(summons.Number.Split('-').Last(), out var index))
                continue;

            summons.Reason = MilitaryReasons[(Math.Max(index, 1) - 1) % MilitaryReasons.Length];
            summons.Comment = "Синтетическая запись учебного реестра повесток военного учёта";
        }

        await db.SaveChangesAsync();
    }
}
