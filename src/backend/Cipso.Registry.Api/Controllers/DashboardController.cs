using Cipso.Registry.Api.Data;
using Cipso.Registry.Api.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cipso.Registry.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/dashboard")]
public sealed class DashboardController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var statusCounts = await db.Summonses
            .AsNoTracking()
            .GroupBy(x => x.Status)
            .Select(group => new
            {
                Status = group.Key,
                Count = group.Count()
            })
            .ToListAsync();

        var recent = await db.Summonses
            .AsNoTracking()
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

        return Ok(new
        {
            Total = await db.Summonses.CountAsync(),
            Active = await db.Summonses.CountAsync(x => activeStatuses.Contains(x.Status)),
            Completed = await db.Summonses.CountAsync(x => x.Status == SummonsStatus.Completed),
            Cancelled = await db.Summonses.CountAsync(x => x.Status == SummonsStatus.Cancelled),
            Citizens = await db.Citizens.CountAsync(),
            Appeals = await db.Appeals.CountAsync(),
            Documents = await db.Documents.CountAsync(),
            ByStatus = statusCounts.ToDictionary(x => x.Status.ToString(), x => x.Count),
            Recent = recent
        });
    }
}
