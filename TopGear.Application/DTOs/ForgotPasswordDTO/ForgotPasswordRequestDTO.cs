using TopGear.Domain.Enums;

namespace TopGear.Application.DTOs.ForgotPasswordDTO;

public class ForgotPasswordRequestDTO
{
    public Guid RequestId { get; set; } = Guid.Empty;
    public string UserEmail { get; set; } = null!;
    public string VerificationCode { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow;
    public ForgotPasswordStatus Status { get; set; } = ForgotPasswordStatus.Pending;
}
