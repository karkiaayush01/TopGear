using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Text;
using TopGear.Application.DTOs.EmailDTO;
using TopGear.Application.DTOs.PartSaleDTO;
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

    public async Task SendSalesInvoiceEmailAsync(string recipientEmail, string customerName, PartSaleDTO sale)
    {
        string emailBody = BuildSalesInvoiceEmail(customerName, sale);

        await SendMailAsync(new SendEmailDTO
        {
            Recipients = new List<string> { recipientEmail },
            Subject = $"Your TopGear Sales Invoice #{sale.SaleId.ToString()[..8].ToUpper()}",
            Body = emailBody,
            IsHtml = true
        });
    }

    private string BuildForgotPasswordEmail(string code)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Email", "Templates", "ForgotPassword.html");
        var html = File.ReadAllText(path);
        return html.Replace("{{VERIFICATION_CODE}}", code);
    }

    public async Task SendLowStockAlertAsync(string adminEmail, IEnumerable<(string PartName, int Quantity)> lowStockParts)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Email", "Templates", "LowStockAlert.html");
        var html = File.ReadAllText(path);

        var rows = new StringBuilder();
        foreach (var (name, qty) in lowStockParts)
        {
            var color = qty == 0 ? "#c62828" : "#e65100";
            rows.AppendLine($"""
                <tr>
                    <td style="padding:10px 12px; border-bottom:1px solid #f0f0f0;">{name}</td>
                    <td style="padding:10px 12px; border-bottom:1px solid #f0f0f0; text-align:center; font-weight:700; color:{color};">{qty}</td>
                </tr>
            """);
        }

        var body = html.Replace("{{PARTS_TABLE}}", rows.ToString())
                       .Replace("{{CHECKED_AT}}", DateTime.UtcNow.ToString("dd MMM yyyy HH:mm") + " UTC");

        await SendMailAsync(new SendEmailDTO
        {
            Recipients = new List<string> { adminEmail },
            Subject = "⚠️ TopGear Low Stock Alert",
            Body = body,
            IsHtml = true
        });
    }

    public async Task SendOverdueCreditReminderAsync(
        string customerEmail,
        string customerName,
        IEnumerable<(Guid SaleId, DateTime SaleDate, decimal AmountDue)> overdueSales)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Email", "Templates", "OverdueCreditReminder.html");
        var html = File.ReadAllText(path);

        var rows = new StringBuilder();
        decimal totalDue = 0;
        foreach (var (saleId, saleDate, amount) in overdueSales)
        {
            totalDue += amount;
            rows.AppendLine($"""
                <tr>
                    <td style="padding:10px 12px; border-bottom:1px solid #f0f0f0;">{saleId.ToString()[..8].ToUpper()}</td>
                    <td style="padding:10px 12px; border-bottom:1px solid #f0f0f0; text-align:center;">{saleDate:dd MMM yyyy}</td>
                    <td style="padding:10px 12px; border-bottom:1px solid #f0f0f0; text-align:right;">NPR {amount:N2}</td>
                </tr>
            """);
        }

        var body = html
            .Replace("{{CUSTOMER_NAME}}", customerName)
            .Replace("{{INVOICES_TABLE}}", rows.ToString())
            .Replace("{{TOTAL_DUE}}", totalDue.ToString("N2"));

        await SendMailAsync(new SendEmailDTO
        {
            Recipients = new List<string> { customerEmail },
            Subject = "TopGear – Outstanding Credit Balance Reminder",
            Body = body,
            IsHtml = true
        });
    }

    private string BuildSalesInvoiceEmail(string customerName, PartSaleDTO sale)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Email", "Templates", "SalesInvoice.html");
        var html = File.ReadAllText(path);

        var itemsTable = new StringBuilder();
        foreach (var item in sale.Items)
        {
            itemsTable.AppendLine($"""
                <tr>
                    <td style="padding:10px 12px; border-bottom:1px solid #f0f0f0;">{item.PartName}</td>
                    <td style="padding:10px 12px; border-bottom:1px solid #f0f0f0; text-align:center;">{item.Quantity}</td>
                    <td style="padding:10px 12px; border-bottom:1px solid #f0f0f0; text-align:right;">NPR {item.UnitPrice:N2}</td>
                    <td style="padding:10px 12px; border-bottom:1px solid #f0f0f0; text-align:right;">NPR {item.TotalPrice:N2}</td>
                </tr>
            """);
        }

        var discountRow = sale.DiscountAmount > 0
            ? $"""<tr><td colspan="3" style="padding:8px 12px; text-align:right; color:#2e7d32;">10% Loyalty Discount:</td><td style="padding:8px 12px; text-align:right; color:#2e7d32;">- NPR {sale.DiscountAmount:N2}</td></tr>"""
            : "";

        var paymentBadge = sale.IsCredit
            ? (sale.IsPaid ? "Credit (Paid)" : "Credit (Due)")
            : "Cash";

        return html
            .Replace("{{CUSTOMER_NAME}}", customerName)
            .Replace("{{SALE_ID}}", sale.SaleId.ToString()[..8].ToUpper())
            .Replace("{{SALE_DATE}}", sale.SaleDate.ToString("dd MMM yyyy"))
            .Replace("{{ITEMS_TABLE}}", itemsTable.ToString())
            .Replace("{{SUBTOTAL}}", sale.SubTotal.ToString("N2"))
            .Replace("{{DISCOUNT_ROW}}", discountRow)
            .Replace("{{TOTAL}}", sale.FinalAmount.ToString("N2"))
            .Replace("{{PAYMENT_TYPE}}", paymentBadge);
    }
}
