using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using TopGear.Application.DTOs.EmailDTO;
using TopGear.Application.Interfaces;
using TopGear.Infrastructure.Config;

namespace TopGear.Infrastructure.Email;

public class EmailService: IEmailService
{
    private readonly SmtpClient _client;
    private string _emailSender;

    public EmailService(IOptions<EmailSettings> options)
    {
        var settings = options.Value;

        _client = new SmtpClient(settings.Host)
        {
            Port = settings.Port,
            Credentials = new NetworkCredential(settings.Email, settings.Password),
            EnableSsl = true
        };

        _emailSender = settings.Email;
    }

    public async Task SendMailAsync(SendEmailDTO data)
    {
        var message = new MailMessage {
            From = new MailAddress(_emailSender, "TopGear"),
            Subject = data.Subject,
            Body = data.Body,
            IsBodyHtml = data.IsHtml
        };

        foreach (var email in data.Recipients)
        {
            message.To.Add(new MailAddress(email));
        }

        await _client.SendMailAsync(message);
    }
}
