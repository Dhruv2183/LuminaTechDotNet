using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LuminaTech.Data;
using LuminaTech.Services;
using System.Security.Claims;

namespace LuminaTech.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly LuminaTechDbContext _db;

    public AuthController(IAuthService authService, LuminaTechDbContext db)
    {
        _authService = authService;
        _db = db;
    }

    [HttpGet("google")]
    public IActionResult GoogleLogin()
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = "/api/auth/callback"
        };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("callback")]
    public async Task<IActionResult> GoogleCallback()
    {
        var authenticateResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

        if (!authenticateResult.Succeeded)
            return Redirect("/login.html?error=auth_failed");

        var claims = authenticateResult.Principal?.Claims;
        var googleId = claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "";
        var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? "";
        var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "";
        var picture = claims?.FirstOrDefault(c => c.Type == "urn:google:picture")?.Value ?? "";

        var user = await _authService.FindOrCreateUser(googleId, email, name, picture);
        var token = _authService.GenerateJwtToken(user);

        SetJwtCookie(token);
        return Redirect("/dashboard.html");
    }

    /// <summary>
    /// Demo login with email + password (for showcasing without Google OAuth)
    /// </summary>
    [HttpPost("demo-login")]
    public async Task<IActionResult> DemoLogin([FromBody] DemoLoginRequest request)
    {
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            return BadRequest(new { error = "Email and password are required" });

        var email = request.Email.ToLower().Trim();

        // Real admin account — full unrestricted access (NOT shown on login page)
        if (email == "superadmin@luminatech.com" && request.Password == "LuminaAdmin@2024!")
        {
            var realAdmin = await _db.Users.FirstOrDefaultAsync(u =>
                u.Email.ToLower() == "superadmin@luminatech.com" && u.IsActive);
            if (realAdmin == null)
                return Unauthorized(new { error = "Admin account not found" });

            var token = _authService.GenerateJwtToken(realAdmin);
            SetJwtCookie(token);
            return Ok(new
            {
                message = "Login successful",
                redirect = "/admin.html",
                name = realAdmin.FullName,
                role = realAdmin.Role
            });
        }

        // Demo accounts — BOTH are sandboxed (read-only, cannot modify real data)
        string? demoPassword = null;
        if (email == "admin@luminatech.com") demoPassword = "Admin@123";
        else if (email == "demo@luminatech.com") demoPassword = "Demo@123";

        if (demoPassword != null && request.Password == demoPassword)
        {
            var demoUser = await _db.Users.FirstOrDefaultAsync(u =>
                u.Email.ToLower() == email && u.IsActive);
            if (demoUser == null)
                return Unauthorized(new { error = "Account not found" });

            var token = _authService.GenerateJwtToken(demoUser, isDemo: true);
            SetJwtCookie(token);
            return Ok(new
            {
                message = "Login successful (Demo Mode — read-only)",
                redirect = demoUser.Role == "Admin" ? "/admin.html" : "/dashboard.html",
                name = demoUser.FullName,
                role = demoUser.Role,
                isDemo = true
            });
        }

        return Unauthorized(new { error = "Invalid email or password" });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("jwt");
        return Ok(new { message = "Logged out successfully" });
    }

    [HttpGet("status")]
    public IActionResult Status()
    {
        if (HttpContext.User.Identity?.IsAuthenticated == true)
        {
            return Ok(new
            {
                authenticated = true,
                userId = HttpContext.User.FindFirst("userId")?.Value,
                email = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value,
                name = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value,
                role = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value,
                profilePicture = HttpContext.User.FindFirst("profilePicture")?.Value
            });
        }
        return Ok(new { authenticated = false });
    }

    private void SetJwtCookie(string token)
    {
        Response.Cookies.Append("jwt", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = false, // false for HTTP localhost
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddHours(1)
        });
    }
}

public class DemoLoginRequest
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}
