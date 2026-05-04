namespace TopGear.Application.DTOs.Report;

public class PurchaseInvoiceReportDTO
{
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public int TotalInvoices { get; set; }
    public decimal TotalAmountSpent { get; set; }
    public int TotalUnitsPurchased { get; set; }
    public List<VendorPurchaseSummary> ByVendor { get; set; } = new();
    public List<PartPurchaseSummary> ByPart { get; set; } = new();
}

public class VendorPurchaseSummary
{
    public Guid VendorId { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public int InvoiceCount { get; set; }
    public decimal TotalAmount { get; set; }
}

public class PartPurchaseSummary
{
    public Guid PartId { get; set; }
    public string PartName { get; set; } = string.Empty;
    public int TotalQuantity { get; set; }
    public decimal TotalAmount { get; set; }
}
