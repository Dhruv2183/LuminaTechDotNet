using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LuminaTech.Data.Entities;

public class ContactSubmission
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Company { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    [MaxLength(100)]
    public string ProductInterest { get; set; } = string.Empty;

    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    public bool IsRead { get; set; } = false;
}
