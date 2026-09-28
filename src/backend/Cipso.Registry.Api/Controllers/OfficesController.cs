using Cipso.Registry.Api.Auth;
using Cipso.Registry.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cipso.Registry.Api.Controllers;

[ApiController]
[Authorize(Roles = "Operator,Manager,Observer")]
[Route("api/offices")]
public sealed class OfficesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        if (User.IsObserver())
        {
            var registryNumber = User.GetCitizenRegistryNumber();
            if (string.IsNullOrWhiteSpace(registryNumber))
                return Forbid();

            var officeIds = await db.Summonses
                .AsNoTracking()
                .Where(x => x.Citizen.RegistryNumber == registryNumber)
                .Select(x => x.AuthorityOfficeId)
                .Distinct()
                .ToListAsync();

            var offices = await db.AuthorityOffices
                .AsNoTracking()
                .Where(x => officeIds.Contains(x.Id))
                .OrderBy(x => x.Name)
                .Select(x => new
                {
                    x.Id,
                    x.Code,
                    x.Name,
                    x.Region
                })
                .ToListAsync();

            return Ok(offices);
        }

        return Ok(await db.AuthorityOffices
            .Include(x => x.Employees)
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync());
    }
}
