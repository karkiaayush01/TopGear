using TopGear.Application.DTOs.Report;

namespace TopGear.Application.Interfaces;

public interface IReportService
{
    Task<PurchaseInvoiceReportDTO> GetPurchaseInvoiceReportAsync(DateTime? from, DateTime? to);
}
