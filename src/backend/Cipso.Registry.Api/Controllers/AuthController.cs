using Cipso.Registry.Api.Auth;
using Cipso.Registry.Api.Contracts;
using Cipso.Registry.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cipso.Registry.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(AppDbContext db, JwtTokenService tokens) : ControllerBase
{
    private static readonly HashSet<string> SupportedProviders =
        new(StringComparer.OrdinalIgnoreCase) { "MAX", "Gosuslugi" };

    [AllowAnonymous]
    [HttpGet("stub/accounts")]
    public async Task<IActionResult> GetStubAccounts()
    {
        var accounts = await db.SystemAccounts
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Role)
            .ThenBy(x => x.DisplayName)
            .Select(x => new ExternalAuthStubAccountResponse(
                x.Id,
                x.DisplayName,
                x.Role,
                x.CitizenRegistryNumber))
            .ToListAsync();

        return Ok(accounts);
    }

    [AllowAnonymous]
    [HttpPost("stub/external")]
    public async Task<IActionResult> ExternalStubLogin(ExternalAuthStubRequest request)
    {
        var provider = request.Provider.Trim();
        if (!SupportedProviders.Contains(provider))
            return BadRequest(new { message = "Поддерживаются только заглушки MAX и Госуслуги («Госключ»)." });

        var account = await db.SystemAccounts
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.AccountId && x.IsActive);

        if (account is null)
            return Unauthorized(new { message = "Учётная запись не найдена или отключена." });

        var normalizedProvider = provider.Equals("MAX", StringComparison.OrdinalIgnoreCase)
            ? "MAX"
            : "Gosuslugi";

        var (token, expiresAt) = tokens.CreateToken(account, normalizedProvider);

        return Ok(new LoginResponse(
            token,
            account.Id,
            account.DisplayName,
            account.Role,
            normalizedProvider,
            expiresAt));
    }
}
