using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LuminaTech.Data;

namespace LuminaTech.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly LuminaTechDbContext _context;

    public AdminController(LuminaTechDbContext context)
    {
        _context = context;
    }

    private bool IsDemoUser() =>
        HttpContext.User.FindFirst("isDemo")?.Value == "true";

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _context.Users
            .Select(u => new
            {
                u.Id,
                u.Email,
                u.FullName,
                u.Role,
                u.IsActive,
                u.CreatedAt,
                u.ProfilePictureUrl
            })
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();

        return Ok(users);
    }

    [HttpPut("users/{id}/role")]
    public async Task<IActionResult> UpdateRole(int id, [FromBody] RoleUpdateDto dto)
    {
        if (IsDemoUser())
            return StatusCode(403, new { message = "Demo accounts cannot modify user roles" });

        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        user.Role = dto.Role;
        await _context.SaveChangesAsync();
        return Ok(new { message = $"User role updated to {dto.Role}" });
    }

    [HttpPut("users/{id}/toggle")]
    public async Task<IActionResult> ToggleActive(int id)
    {
        if (IsDemoUser())
            return StatusCode(403, new { message = "Demo accounts cannot modify users" });

        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        user.IsActive = !user.IsActive;
        await _context.SaveChangesAsync();
        return Ok(new { message = $"User {(user.IsActive ? "activated" : "deactivated")}" });
    }

    [HttpGet("contacts")]
    public async Task<IActionResult> GetContactSubmissions()
    {
        var submissions = await _context.ContactSubmissions
            .OrderByDescending(c => c.SubmittedAt)
            .ToListAsync();
        return Ok(submissions);
    }

    [HttpPut("contacts/{id}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var submission = await _context.ContactSubmissions.FindAsync(id);
        if (submission == null) return NotFound();

        submission.IsRead = true;
        await _context.SaveChangesAsync();
        return Ok(new { message = "Marked as read" });
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        return Ok(new
        {
            totalUsers = await _context.Users.CountAsync(),
            activeUsers = await _context.Users.CountAsync(u => u.IsActive),
            totalProducts = await _context.Products.CountAsync(p => p.IsActive),
            totalContacts = await _context.ContactSubmissions.CountAsync(),
            unreadContacts = await _context.ContactSubmissions.CountAsync(c => !c.IsRead)
        });
    }
}

public class RoleUpdateDto
{
    public string Role { get; set; } = "User";
}
