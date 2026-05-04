using TopGear.Application.DTOs.Report;
using TopGear.Application.Interfaces;

namespace TopGear.Application.Services;

public class ReportService : IReportService
{
    private readonly IPurchaseInvoiceRepository _invoiceRepository;

    public ReportService(IPurchaseInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<PurchaseInvoiceReportDTO> GetPurchaseInvoiceReportAsync(DateTime? from, DateTime? to)
    {
        var invoices = await _invoiceRepository.GetForReportAsync(from, to);

        var allItems = invoices.SelectMany(i => i.Items).ToList();

        var byVendor = invoices
            .GroupBy(i => new { i.VendorId, VendorName = i.Vendor?.VendorName ?? "" })
            .Select(g => new VendorPurchaseSummary
            {
                VendorId = g.Key.VendorId,
                VendorName = g.Key.VendorName,
                InvoiceCount = g.Count(),
                TotalAmount = g.SelectMany(i => i.Items).Sum(item => item.Quantity * item.UnitPrice)
            })
            .ToList();

        var byPart = allItems
            .GroupBy(i => new { i.PartId, PartName = i.Part?.PartName ?? "" })
            .Select(g => new PartPurchaseSummary
            {
                PartId = g.Key.PartId,
                PartName = g.Key.PartName,
                TotalQuantity = g.Sum(i => i.Quantity),
                TotalAmount = g.Sum(i => i.Quantity * i.UnitPrice)
            })
            .ToList();

        return new PurchaseInvoiceReportDTO
        {
            From = from,
            To = to,
            TotalInvoices = invoices.Count,
            TotalAmountSpent = allItems.Sum(i => i.Quantity * i.UnitPrice),
            TotalUnitsPurchased = allItems.Sum(i => i.Quantity),
            ByVendor = byVendor,
            ByPart = byPart
        };
    }
}
