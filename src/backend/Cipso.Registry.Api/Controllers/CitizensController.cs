using Cipso.Registry.Api.Auth;
using Cipso.Registry.Api.Contracts;
using Cipso.Registry.Api.Data;
using Cipso.Registry.Api.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cipso.Registry.Api.Controllers;

[ApiController]
[Authorize(Roles = "Operator,Manager,Observer,AutomationEngineer")]
[Route("api/citizens")]
public sealed class CitizensController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search)
    {
        var query = db.Citizens
            .Include(x => x.Address)
            .AsNoTracking()
            .Where(x => !x.IsWrittenOff);

        if (User.IsObserver())
        {
            var registryNumber = User.GetCitizenRegistryNumber();
            if (string.IsNullOrWhiteSpace(registryNumber))
                return Forbid();

            query = query.Where(x => x.RegistryNumber == registryNumber);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            query = query.Where(x =>
                x.RegistryNumber.ToLower().Contains(s) ||
                x.LastName.ToLower().Contains(s) ||
                x.FirstName.ToLower().Contains(s));
        }

        var items = await query
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .Take(100)
            .ToListAsync();

        return Ok(items);
    }

    [Authorize(Roles = "Operator,Manager")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateCitizenRequest request)
    {
        if (await db.Citizens.AnyAsync(x => x.RegistryNumber == request.RegistryNumber))
            return Conflict(new { message = "Registry number already exists." });

        var citizen = new Citizen
        {
            RegistryNumber = request.RegistryNumber.Trim(),
            LastName = request.LastName.Trim(),
            FirstName = request.FirstName.Trim(),
            MiddleName = request.MiddleName?.Trim(),
            BirthDate = request.BirthDate,
            Email = request.Email?.Trim(),
            Phone = request.Phone?.Trim(),
            Address = new Address
            {
                PostalCode = request.Address.PostalCode.Trim(),
                Region = request.Address.Region.Trim(),
                City = request.Address.City.Trim(),
                Street = request.Address.Street.Trim(),
                Building = request.Address.Building.Trim(),
                Apartment = request.Address.Apartment?.Trim()
            }
        };

        db.Citizens.Add(citizen);
        await db.SaveChangesAsync();

        return Created($"/api/citizens/{citizen.Id}", citizen);
    }

    [Authorize(Roles = "Operator,Manager,AutomationEngineer")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> WriteOff(Guid id)
    {
        var citizen = await db.Citizens.FirstOrDefaultAsync(x => x.Id == id);
        if (citizen is null)
            return NotFound(new { message = "Призывник не найден." });

        if (citizen.IsWrittenOff)
            return NoContent();

        citizen.IsWrittenOff = true;
        citizen.WrittenOffAt = DateTimeOffset.UtcNow;
        citizen.WrittenOffBy = User.Identity?.Name ?? "system";

        var linkedAccounts = await db.SystemAccounts
            .Where(x => x.CitizenRegistryNumber == citizen.RegistryNumber && x.IsActive)
            .ToListAsync();

        foreach (var account in linkedAccounts)
            account.IsActive = false;

        await db.SaveChangesAsync();
        return NoContent();
    }
}
