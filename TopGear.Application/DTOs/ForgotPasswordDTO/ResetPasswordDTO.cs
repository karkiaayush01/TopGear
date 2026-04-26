namespace TopGear.Application.DTOs.ForgotPasswordDTO;

public class ResetPasswordDTO
{
    public string Email { get; set; } = null!;
    public string VerificationCode { get; set; } = null!;
    public string Password { get; set; } = null!;
}
