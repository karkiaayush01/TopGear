using TopGear.Application.DTOs.EmailDTO;

namespace TopGear.Application.Interfaces;

public interface IEmailService
{
    Task SendForgotPasswordEmail(string email, string verificationCode);
}
