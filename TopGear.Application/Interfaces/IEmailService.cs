using TopGear.Application.DTOs.EmailDTO;

namespace TopGear.Application.Interfaces;

public interface IEmailService
{
    Task SendMailAsync(SendEmailDTO data);
}
