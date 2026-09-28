using Cipso.Registry.Api.Auth;
using Cipso.Registry.Api.Data;
using Cipso.Registry.Api.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cipso.Registry.Api.Controllers;

[ApiController]
[Authorize(Roles = "Operator,Manager,Observer")]
[Route("api/dashboard")]
public sealed class DashboardController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var observer = User.IsObserver();
        var registryNumber = observer
            ? User.GetCitizenRegistryNumber()
            : null;

        if (observer && string.IsNullOrWhiteSpace(registryNumber))
            return Forbid();

        var summonsQuery = db.Summonses.AsNoTracking();

        if (observer)
            summonsQuery = summonsQuery.Where(x => x.Citizen.RegistryNumber == registryNumber);

        var statusCounts = await summonsQuery
            .GroupBy(x => x.Status)
            .Select(group => new
            {
                Status = group.Key,
                Count = group.Count()
            })
            .ToListAsync();

        var recent = await summonsQuery
            .Include(x => x.Citizen)
            .Include(x => x.AuthorityOffice)
            .OrderByDescending(x => x.IssuedAt)
            .ThenByDescending(x => x.DueAt)
            .Take(5)
            .Select(x => new
            {
                x.Id,
                x.Number,
                x.Status,
                x.IssuedAt,
                x.DueAt,
                Citizen = new
                {
                    x.Citizen.Id,
                    x.Citizen.RegistryNumber,
                    x.Citizen.LastName,
                    x.Citizen.FirstName
                },
                Office = new
                {
                    x.AuthorityOffice.Id,
                    x.AuthorityOffice.Code,
                    x.AuthorityOffice.Name
                }
            })
            .ToListAsync();

        var activeStatuses = new[]
        {
            SummonsStatus.Issued,
            SummonsStatus.Delivered,
            SummonsStatus.Acknowledged
        };

        var citizens = observer
            ? await db.Citizens.CountAsync(x => x.RegistryNumber == registryNumber)
            : await db.Citizens.CountAsync();

        var appeals = observer
            ? await db.Appeals.CountAsync(x => x.Summons.Citizen.RegistryNumber == registryNumber)
            : await db.Appeals.CountAsync();

        var documents = observer
            ? await db.Documents.CountAsync(x => x.Summons.Citizen.RegistryNumber == registryNumber)
            : await db.Documents.CountAsync();

        return Ok(new
        {
            Total = await summonsQuery.CountAsync(),
            Active = await summonsQuery.CountAsync(x => activeStatuses.Contains(x.Status)),
            Completed = await summonsQuery.CountAsync(x => x.Status == SummonsStatus.Completed),
            Cancelled = await summonsQuery.CountAsync(x => x.Status == SummonsStatus.Cancelled),
            Citizens = citizens,
            Appeals = appeals,
            Documents = documents,
            ByStatus = statusCounts.ToDictionary(x => x.Status.ToString(), x => x.Count),
            Recent = recent
        });
    }
}
