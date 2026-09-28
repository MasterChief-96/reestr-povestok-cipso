using Cipso.Registry.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cipso.Registry.Api.Controllers;

[ApiController]
[Authorize(Roles = "Operator,Manager,Observer")]
[Route("api/appeals")]
public sealed class AppealsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await db.Appeals
            .AsNoTracking()
            .Include(x => x.Summons)
            .ThenInclude(x => x.Citizen)
            .OrderByDescending(x => x.SubmittedAt)
            .Take(200)
            .Select(x => new
            {
                x.Id,
                x.Type,
                x.Text,
                x.Status,
                x.SubmittedAt,
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
