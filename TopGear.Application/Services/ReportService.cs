using TopGear.Application.DTOs.Report;
using TopGear.Application.Interfaces;

namespace TopGear.Application.Services;

public class ReportService : IReportService
{
    private readonly IPurchaseInvoiceRepository _invoiceRepository;
    private readonly IPartSaleRepository _saleRepository;

    public ReportService(IPurchaseInvoiceRepository invoiceRepository, IPartSaleRepository saleRepository)
    {
        _invoiceRepository = invoiceRepository;
        _saleRepository = saleRepository;
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

    public async Task<FinancialReportDTO> GetFinancialReportAsync(string period, DateTime? from, DateTime? to)
    {
        var sales = await _saleRepository.GetSalesForReportAsync(from, to);

        var normalizedPeriod = period.ToLower();

        var byPeriod = normalizedPeriod switch
        {
            "monthly" => sales
                .GroupBy(s => s.SaleDate.ToString("yyyy-MM"))
                .Select(g => new PeriodRevenueSummary
                {
                    Label = g.Key,
                    SaleCount = g.Count(),
                    Revenue = g.Sum(s => s.FinalAmount),
                    Discount = g.Sum(s => s.DiscountAmount)
                }).ToList(),
            "yearly" => sales
                .GroupBy(s => s.SaleDate.ToString("yyyy"))
                .Select(g => new PeriodRevenueSummary
                {
                    Label = g.Key,
                    SaleCount = g.Count(),
                    Revenue = g.Sum(s => s.FinalAmount),
                    Discount = g.Sum(s => s.DiscountAmount)
                }).ToList(),
            _ => sales
                .GroupBy(s => s.SaleDate.ToString("yyyy-MM-dd"))
                .Select(g => new PeriodRevenueSummary
                {
                    Label = g.Key,
                    SaleCount = g.Count(),
                    Revenue = g.Sum(s => s.FinalAmount),
                    Discount = g.Sum(s => s.DiscountAmount)
                }).ToList()
        };

        var allItems = sales.SelectMany(s => s.Items).ToList();

        var topSellingParts = allItems
            .GroupBy(i => new { i.PartId, PartName = i.Part?.PartName ?? "" })
            .Select(g => new TopSellingPartSummary
            {
                PartId = g.Key.PartId,
                PartName = g.Key.PartName,
                TotalQuantity = g.Sum(i => i.Quantity),
                TotalRevenue = g.Sum(i => i.TotalPrice)
            })
            .OrderByDescending(p => p.TotalQuantity)
            .Take(10)
            .ToList();

        return new FinancialReportDTO
        {
            From = from,
            To = to,
            Period = normalizedPeriod,
            TotalSales = sales.Count,
            TotalRevenue = sales.Sum(s => s.FinalAmount),
            TotalDiscount = sales.Sum(s => s.DiscountAmount),
            CashRevenue = sales.Where(s => !s.IsCredit).Sum(s => s.FinalAmount),
            CreditRevenue = sales.Where(s => s.IsCredit).Sum(s => s.FinalAmount),
            ByPeriod = byPeriod,
            TopSellingParts = topSellingParts
        };
    }

    public async Task<List<RegularCustomerDTO>> GetRegularCustomersAsync(int minPurchases = 2)
    {
        var sales = await _saleRepository.GetSalesForReportAsync(null, null);

        return sales
            .GroupBy(s => new
            {
                s.CustomerId,
                CustomerName = $"{s.Customer.FirstName} {s.Customer.LastName}",
                Email = s.Customer.Email ?? ""
            })
            .Where(g => g.Count() >= minPurchases)
            .Select(g => new RegularCustomerDTO
            {
                CustomerId = g.Key.CustomerId,
                CustomerName = g.Key.CustomerName,
                Email = g.Key.Email,
                PurchaseCount = g.Count(),
                TotalSpent = g.Sum(s => s.FinalAmount),
                LastPurchaseDate = g.Max(s => s.SaleDate)
            })
            .OrderByDescending(c => c.PurchaseCount)
            .ToList();
    }

    public async Task<List<HighSpenderDTO>> GetHighSpendersAsync(int top = 10)
    {
        var sales = await _saleRepository.GetSalesForReportAsync(null, null);

        var ranked = sales
            .GroupBy(s => new
            {
                s.CustomerId,
                CustomerName = $"{s.Customer.FirstName} {s.Customer.LastName}",
                Email = s.Customer.Email ?? ""
            })
            .Select(g => new HighSpenderDTO
            {
                CustomerId = g.Key.CustomerId,
                CustomerName = g.Key.CustomerName,
                Email = g.Key.Email,
                TotalSpent = g.Sum(s => s.FinalAmount),
                PurchaseCount = g.Count()
            })
            .OrderByDescending(c => c.TotalSpent)
            .Take(top)
            .ToList();

        for (int i = 0; i < ranked.Count; i++)
            ranked[i].Rank = i + 1;

        return ranked;
    }

    public async Task<List<PendingCreditDTO>> GetPendingCreditsAsync()
    {
        var sales = await _saleRepository.GetSalesForReportAsync(null, null);

        return sales
            .Where(s => s.IsCredit && !s.IsPaid)
            .GroupBy(s => new
            {
                s.CustomerId,
                CustomerName = $"{s.Customer.FirstName} {s.Customer.LastName}",
                Email = s.Customer.Email ?? "",
                Phone = s.Customer.PhoneNumber ?? ""
            })
            .Select(g => new PendingCreditDTO
            {
                CustomerId = g.Key.CustomerId,
                CustomerName = g.Key.CustomerName,
                Email = g.Key.Email,
                Phone = g.Key.Phone,
                UnpaidInvoiceCount = g.Count(),
                TotalAmountDue = g.Sum(s => s.FinalAmount),
                OldestUnpaidDate = g.Min(s => s.SaleDate)
            })
            .OrderByDescending(c => c.TotalAmountDue)
            .ToList();
    }
}
