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

        var registryNumber = string.IsNullOrWhiteSpace(request.CitizenRegistryNumber)
            ? null
            : request.CitizenRegistryNumber.Trim();

        if (role == "Observer" && registryNumber is not null &&
            !await db.Citizens.AnyAsync(x => x.RegistryNumber == registryNumber))
        {
            return BadRequest(new { message = "Призывник с таким реестровым номером не найден." });
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

        return Created($"/api/accounts/{account.Id}", account);
    }
}
