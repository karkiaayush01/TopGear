using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TopGear.Domain.Enums;

namespace TopGear.Domain.Entities;

[Index(nameof(UserEmail))]
public class ForgotPasswordRequest
{
    [Key]
    public Guid RequestId { get; set; } = Guid.NewGuid();

    [Required]
    public string UserEmail { get; set; } = null!;

    [Required]
    public string VerificationCode { get; set; } = null!;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddMinutes(15);

    [Required]
    public ForgotPasswordStatus Status { get; set; } = ForgotPasswordStatus.Pending;
}
