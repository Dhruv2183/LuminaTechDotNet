using Microsoft.AspNetCore.Mvc;
using LuminaTech.Data;
using LuminaTech.Data.Entities;

namespace LuminaTech.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactController : ControllerBase
{
    private readonly LuminaTechDbContext _context;

    public ContactController(LuminaTechDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Submit([FromBody] ContactSubmissionDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.FullName) || string.IsNullOrWhiteSpace(dto.Email))
            return BadRequest(new { message = "Name and email are required" });

        var submission = new ContactSubmission
        {
            FullName = dto.FullName,
            Email = dto.Email,
            Company = dto.Company ?? "",
            Message = dto.Message ?? "",
            ProductInterest = dto.ProductInterest ?? "",
            SubmittedAt = DateTime.UtcNow,
            IsRead = false
        };

        _context.ContactSubmissions.Add(submission);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Thank you! We'll be in touch within 24 hours.", id = submission.Id });
    }
}

public class ContactSubmissionDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Company { get; set; }
    public string? Message { get; set; }
    public string? ProductInterest { get; set; }
}
