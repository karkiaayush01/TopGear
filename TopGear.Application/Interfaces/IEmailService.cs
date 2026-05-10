using TopGear.Application.DTOs.PartSaleDTO;

namespace TopGear.Application.Interfaces;

public interface IEmailService
{
    Task SendForgotPasswordEmail(string email, string verificationCode);
    Task SendSalesInvoiceEmailAsync(string recipientEmail, string customerName, PartSaleDTO sale);
    Task SendLowStockAlertAsync(string adminEmail, IEnumerable<(string PartName, int Quantity)> lowStockParts);
    Task SendOverdueCreditReminderAsync(string customerEmail, string customerName, IEnumerable<(Guid SaleId, DateTime SaleDate, decimal AmountDue)> overdueSales);
}
