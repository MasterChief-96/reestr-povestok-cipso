using Cipso.Registry.Api.Auth;
using Cipso.Registry.Api.Contracts;
using Cipso.Registry.Api.Data;
using Cipso.Registry.Api.Domain;
using Cipso.Registry.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cipso.Registry.Api.Controllers;

[ApiController]
[Authorize(Roles = "Operator,Manager,Observer")]
[Route("api/summons")]
public sealed class SummonsController(AppDbContext db, SummonsService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] SummonsStatus? status,
        [FromQuery] Guid? officeId,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = db.Summonses
            .Include(x => x.Citizen)
            .Include(x => x.AuthorityOffice)
            .AsNoTracking();

        if (User.IsObserver())
        {
            var registryNumber = User.GetCitizenRegistryNumber();
            if (string.IsNullOrWhiteSpace(registryNumber))
                return Forbid();

            query = query.Where(x => x.Citizen.RegistryNumber == registryNumber);
        }

        if (status is not null)
            query = query.Where(x => x.Status == status);

        if (officeId is not null)
            query = query.Where(x => x.AuthorityOfficeId == officeId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            query = query.Where(x =>
                x.Number.ToLower().Contains(s) ||
                x.Citizen.RegistryNumber.ToLower().Contains(s) ||
                x.Citizen.LastName.ToLower().Contains(s));
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(x => x.IssuedAt)
            .ThenByDescending(x => x.DueAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new
            {
                x.Id,
                x.Number,
                x.Status,
                x.IssuedAt,
                x.DueAt,
                x.Reason,
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

        return Ok(new
        {
            items,
            page,
            pageSize,
            total,
            totalPages = (int)Math.Ceiling(total / (double)pageSize)
        });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var registryNumber = User.IsObserver()
            ? User.GetCitizenRegistryNumber()
            : null;

        if (User.IsObserver() && string.IsNullOrWhiteSpace(registryNumber))
            return Forbid();

        var query = db.Summonses
            .Include(x => x.Citizen).ThenInclude(x => x.Address)
            .Include(x => x.AuthorityOffice)
            .Include(x => x.CreatedByEmployee)
            .Include(x => x.StatusHistory)
            .Include(x => x.Notifications).ThenInclude(x => x.DeliveryAttempts)
            .Include(x => x.Appeals)
            .Include(x => x.Documents)
            .Include(x => x.AuditEvents)
            .AsNoTracking();

        if (User.IsObserver())
            query = query.Where(x => x.Citizen.RegistryNumber == registryNumber);

        var item = await query.FirstOrDefaultAsync(x => x.Id == id);

        return item is null ? NotFound() : Ok(item);
    }

    [Authorize(Roles = "Operator,Manager")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateSummonsRequest request)
    {
        try
        {
            var entity = await service.CreateAsync(request);
            return CreatedAtAction(
                nameof(GetById),
                new { id = entity.Id },
                new { entity.Id, entity.Number, entity.Status });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "Operator,Manager")]
    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus(Guid id, ChangeStatusRequest request)
    {
        try
        {
            var entity = await service.ChangeStatusAsync(id, request);
            return Ok(new { entity.Id, entity.Status });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "Operator,Manager")]
    [HttpPost("{id:guid}/notifications")]
    public async Task<IActionResult> SendNotification(Guid id, CreateNotificationRequest request)
    {
        try
        {
            var notification = await service.SendMockNotificationAsync(id, request);
            return Ok(new
            {
                notification.Id,
                notification.Channel,
                notification.Status,
                notification.DestinationMasked
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "Operator,Manager")]
    [HttpPost("{id:guid}/appeals")]
    public async Task<IActionResult> CreateAppeal(Guid id, CreateAppealRequest request)
    {
        var summons = await db.Summonses.FindAsync(id);
        if (summons is null)
            return NotFound();

        if (string.IsNullOrWhiteSpace(request.Text))
            return BadRequest(new { message = "Appeal text is required." });

        var appeal = new Appeal
        {
            Summons = summons,
            Type = request.Type.Trim(),
            Text = request.Text.Trim(),
            Status = AppealStatus.Submitted
        };

        db.Appeals.Add(appeal);
        db.AuditEvents.Add(new AuditEvent
        {
            Summons = summons,
            Action = "AppealSubmitted",
            Actor = request.Actor.Trim(),
            Details = request.Type.Trim()
        });

        await db.SaveChangesAsync();
        return Ok(appeal);
    }
}
