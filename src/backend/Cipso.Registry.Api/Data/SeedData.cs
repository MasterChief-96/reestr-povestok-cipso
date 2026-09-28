using Cipso.Registry.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Cipso.Registry.Api.Data;

public static class SeedData
{
    public static async Task InitializeAsync(AppDbContext db)
    {
        if (await db.AuthorityOffices.AnyAsync()) return;

        var office = new AuthorityOffice
        {
            Code = "TEST-01",
            Name = "Учебное подразделение №1",
            Region = "Тестовый регион"
        };

        var employee = new Employee
        {
            AuthorityOffice = office,
            PersonnelNumber = "EMP-0001",
            FullName = "Иванов Иван Оператор",
            Role = "Operator"
        };

        var citizen = new Citizen
        {
            RegistryNumber = "TEST-0001",
            LastName = "Сидоров",
            FirstName = "Сидор",
            MiddleName = "Сидорович",
            BirthDate = new DateOnly(2000, 1, 1),
            Email = "demo@example.test",
            Phone = "+70000000000",
            Address = new Address
            {
                PostalCode = "000000",
                Region = "Тестовый регион",
                City = "Тестоград",
                Street = "Учебная",
                Building = "1",
                Apartment = "1"
            }
        };

        var summons = new Summons
        {
            Number = "CIPSO-2026-0001",
            Citizen = citizen,
            AuthorityOffice = office,
            CreatedByEmployee = employee,
            IssuedAt = DateOnly.FromDateTime(DateTime.UtcNow.Date),
            DueAt = DateTimeOffset.UtcNow.AddDays(7),
            Reason = "Учебное демонстрационное оповещение",
            Status = SummonsStatus.Issued,
            Comment = "Синтетические данные"
        };

        summons.StatusHistory.Add(new SummonsStatusHistory
        {
            FromStatus = SummonsStatus.Draft,
            ToStatus = SummonsStatus.Issued,
            ChangedBy = "seed",
            Comment = "Начальный статус"
        });
        summons.AuditEvents.Add(new AuditEvent
        {
            Action = "SummonsCreated",
            Actor = "seed",
            Details = "Synthetic demo record"
        });

        db.AddRange(office, employee, citizen, summons);
        await db.SaveChangesAsync();
    }
}
