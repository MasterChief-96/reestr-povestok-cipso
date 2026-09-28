using Cipso.Registry.Api.Auth;
using Cipso.Registry.Api.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cipso.Registry.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(JwtTokenService tokens) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        var user = tokens.ValidateCredentials(request.Username.Trim(), request.Password);
        if (user is null)
            return Unauthorized(new { message = "Invalid username or password." });

        var (token, expiresAt) = tokens.CreateToken(user);
        return Ok(new LoginResponse(
            token,
            user.Username,
            user.DisplayName,
            user.Role,
            expiresAt));
    }
}
