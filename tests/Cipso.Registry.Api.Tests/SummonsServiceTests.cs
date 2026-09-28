using Cipso.Registry.Api.Contracts;
using Cipso.Registry.Api.Data;
using Cipso.Registry.Api.Domain;
using Cipso.Registry.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace Cipso.Registry.Api.Tests;

public sealed class SummonsServiceTests
{
    [Fact]
    public async Task CreateAsync_RejectsDueDateBeforeIssuedDate()
    {
        await using var db = CreateDb();
        var service = new SummonsService(db);

        var request = new CreateSummonsRequest(
            "CIPSO-T-001",
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            new DateOnly(2026, 9, 28),
            new DateTimeOffset(2026, 9, 27, 10, 0, 0, TimeSpan.Zero),
            "Test",
            null);

        var error = await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(request));
        Assert.Contains("DueAt", error.Message);
    }

    [Fact]
    public async Task CreateAsync_WritesInitialHistoryAndAudit()
    {
        await using var db = CreateDb();
        var (citizen, office, employee) = await AddReferencesAsync(db);
        var service = new SummonsService(db);

        var request = new CreateSummonsRequest(
            "CIPSO-T-002",
            citizen.Id,
            office.Id,
            employee.Id,
            new DateOnly(2026, 9, 28),
            new DateTimeOffset(2026, 10, 1, 10, 0, 0, TimeSpan.Zero),
            "Synthetic test",
            "Unit test");

        var summons = await service.CreateAsync(request);

        Assert.Equal(SummonsStatus.Issued, summons.Status);
        Assert.Single(summons.StatusHistory);
        Assert.Single(summons.AuditEvents);
        Assert.Equal("SummonsCreated", summons.AuditEvents[0].Action);
        Assert.Equal("EMP-T-001", summons.StatusHistory[0].ChangedBy);
    }

    [Fact]
    public async Task ChangeStatusAsync_RejectsCurrentStatus()
    {
        await using var db = CreateDb();
        var (citizen, office, employee) = await AddReferencesAsync(db);

        var summons = new Summons
        {
            Number = "CIPSO-T-003",
            Citizen = citizen,
            AuthorityOffice = office,
            CreatedByEmployee = employee,
            IssuedAt = new DateOnly(2026, 9, 28),
            DueAt = new DateTimeOffset(2026, 10, 1, 10, 0, 0, TimeSpan.Zero),
            Reason = "Synthetic test",
            Status = SummonsStatus.Issued
        };

        db.Summonses.Add(summons);
        await db.SaveChangesAsync();

        var service = new SummonsService(db);
        var request = new ChangeStatusRequest(SummonsStatus.Issued, "tester", null);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.ChangeStatusAsync(summons.Id, request));
    }

    private static AppDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static async Task<(Citizen Citizen, AuthorityOffice Office, Employee Employee)> AddReferencesAsync(AppDbContext db)
    {
        var citizen = new Citizen
        {
            RegistryNumber = "TEST-CITIZEN",
            LastName = "Тестов",
            FirstName = "Тест",
            BirthDate = new DateOnly(2000, 1, 1),
            Address = new Address
            {
                PostalCode = "000000",
                Region = "Test",
                City = "Test",
                Street = "Test",
                Building = "1"
            }
        };

        var office = new AuthorityOffice
        {
            Code = "OFF-T",
            Name = "Test office",
            Region = "Test"
        };

        var employee = new Employee
        {
            AuthorityOffice = office,
            PersonnelNumber = "EMP-T-001",
            FullName = "Test Operator",
            Role = "Operator"
        };

        db.AddRange(citizen, office, employee);
        await db.SaveChangesAsync();

        return (citizen, office, employee);
    }
}
