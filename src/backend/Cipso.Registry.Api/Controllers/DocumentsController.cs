using Cipso.Registry.Api.Auth;
using Cipso.Registry.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cipso.Registry.Api.Controllers;

[ApiController]
[Authorize(Roles = "Operator,Manager,Observer")]
[Route("api/documents")]
public sealed class DocumentsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var query = db.Documents
            .AsNoTracking()
            .Include(x => x.Summons)
            .ThenInclude(x => x.Citizen)
            .AsQueryable();

        if (User.IsObserver())
        {
            var registryNumber = User.GetCitizenRegistryNumber();
            if (string.IsNullOrWhiteSpace(registryNumber))
                return Forbid();

            query = query.Where(x =>
                x.Summons.Citizen.RegistryNumber == registryNumber &&
                !x.Summons.Citizen.IsWrittenOff);
        }

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Take(200)
            .Select(x => new
            {
                x.Id,
                x.FileName,
                x.MimeType,
                x.StorageUri,
                x.CreatedAt,
                Summons = new
                {
                    x.Summons.Id,
                    x.Summons.Number,
                    x.Summons.Status
                },
                Citizen = new
                {
                    x.Summons.Citizen.Id,
                    x.Summons.Citizen.RegistryNumber,
                    x.Summons.Citizen.LastName,
                    x.Summons.Citizen.FirstName
                }
            })
            .ToListAsync();

        return Ok(items);
    }
}
