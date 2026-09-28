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
    public async Task<IActionResult> GetAll() => Ok(await db.AuthorityOffices
        .Include(x => x.Employees)
        .AsNoTracking()
        .OrderBy(x => x.Name)
        .ToListAsync());
}
