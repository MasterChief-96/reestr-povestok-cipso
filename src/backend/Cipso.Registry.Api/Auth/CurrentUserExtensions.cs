using System.Security.Claims;

namespace Cipso.Registry.Api.Auth;

public static class CurrentUserExtensions
{
    public static bool IsObserver(this ClaimsPrincipal user) =>
        user.IsInRole("Observer");

    public static string? GetCitizenRegistryNumber(this ClaimsPrincipal user) =>
        user.FindFirst("citizen_registry_number")?.Value;
}
