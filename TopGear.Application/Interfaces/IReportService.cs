using TopGear.Application.DTOs.Report;

namespace TopGear.Application.Interfaces;

public interface IReportService
{
    Task<PurchaseInvoiceReportDTO> GetPurchaseInvoiceReportAsync(DateTime? from, DateTime? to);
    Task<FinancialReportDTO> GetFinancialReportAsync(string period, DateTime? from, DateTime? to);
    Task<List<RegularCustomerDTO>> GetRegularCustomersAsync(int minPurchases = 2);
    Task<List<HighSpenderDTO>> GetHighSpendersAsync(int top = 10);
    Task<List<PendingCreditDTO>> GetPendingCreditsAsync();
}
