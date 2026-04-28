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

    public async Task SendForgotPasswordEmail(string recipientEmail, string verificationCode)
    {
        string emailBody = BuildForgotPasswordEmail(verificationCode);

        await SendMailAsync(new SendEmailDTO
        {
            Recipients = new List<string> { recipientEmail },
            Subject = "Your Forgot Password Request",
            Body = emailBody,
            IsHtml = true
        });
    }

    private async Task SendMailAsync(SendEmailDTO data)
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

    /// <summary>
    /// Build a forgot password email using the verification code and email template
    /// </summary>
    /// <param name="code">Generated verification code</param>
    /// <returns>The html document after updating with verification code</returns>
    private string BuildForgotPasswordEmail(string code)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Email", "Templates", "ForgotPassword.html");
        var html = File.ReadAllText(path);
        return html.Replace("{{VERIFICATION_CODE}}", code);
    }
}
