using Cipso.Registry.Api.Contracts;
using Cipso.Registry.Api.Data;
using Cipso.Registry.Api.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cipso.Registry.Api.Controllers;

[ApiController]
[Authorize(Roles = "AutomationEngineer")]
[Route("api/accounts")]
public sealed class AccountsController(AppDbContext db) : ControllerBase
{
    private static readonly HashSet<string> AllowedRoles =
        new(StringComparer.Ordinal)
        {
            "Operator",
            "Manager",
            "Observer",
            "AutomationEngineer"
        };

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var accounts = await db.SystemAccounts
            .AsNoTracking()
            .OrderBy(x => x.Role)
            .ThenBy(x => x.DisplayName)
            .ToListAsync();

        return Ok(accounts);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateSystemAccountRequest request)
    {
        var role = request.Role.Trim();
        if (!AllowedRoles.Contains(role))
            return BadRequest(new { message = "Неизвестная роль учётной записи." });

        var displayName = request.DisplayName.Trim();
        if (string.IsNullOrWhiteSpace(displayName))
            return BadRequest(new { message = "Отображаемое имя обязательно." });

        await using var transaction = await db.Database.BeginTransactionAsync();

        Citizen? citizen = null;
        string? registryNumber = null;

        if (role == "Observer")
        {
            if (request.Citizen is null)
            {
                return BadRequest(new
                {
                    message = "Для роли «Призывник» необходимо заполнить карточку призывника."
                });
            }

            registryNumber = request.Citizen.RegistryNumber.Trim();

            if (string.IsNullOrWhiteSpace(registryNumber))
                return BadRequest(new { message = "Реестровый номер призывника обязателен." });

            if (await db.Citizens.AnyAsync(x => x.RegistryNumber == registryNumber))
                return Conflict(new { message = "Призывник с таким реестровым номером уже существует." });

            if (await db.SystemAccounts.AnyAsync(x => x.CitizenRegistryNumber == registryNumber))
                return Conflict(new { message = "Для этого призывника уже существует учётная запись." });

            citizen = new Citizen
            {
                RegistryNumber = registryNumber,
                LastName = request.Citizen.LastName.Trim(),
                FirstName = request.Citizen.FirstName.Trim(),
                MiddleName = request.Citizen.MiddleName?.Trim(),
                BirthDate = request.Citizen.BirthDate,
                Email = request.Citizen.Email?.Trim(),
                Phone = request.Citizen.Phone?.Trim(),
                Address = new Address
                {
                    PostalCode = request.Citizen.Address.PostalCode.Trim(),
                    Region = request.Citizen.Address.Region.Trim(),
                    City = request.Citizen.Address.City.Trim(),
                    Street = request.Citizen.Address.Street.Trim(),
                    Building = request.Citizen.Address.Building.Trim(),
                    Apartment = request.Citizen.Address.Apartment?.Trim()
                }
            };

            db.Citizens.Add(citizen);
        }

        var account = new SystemAccount
        {
            ExternalSubject = $"demo-{Guid.NewGuid():N}",
            DisplayName = displayName,
            Role = role,
            CitizenRegistryNumber = registryNumber,
            IsActive = true
        };

        db.SystemAccounts.Add(account);
        await db.SaveChangesAsync();
        await transaction.CommitAsync();

        return Created($"/api/accounts/{account.Id}", account);
    }
}
