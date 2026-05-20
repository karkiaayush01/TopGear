namespace TopGear.Application.DTOs.Report;

public class FinancialReportDTO
{
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public string Period { get; set; } = "daily";
    public int TotalSales { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal CashRevenue { get; set; }
    public decimal CreditRevenue { get; set; }
    public List<PeriodRevenueSummary> ByPeriod { get; set; } = new();
    public List<TopSellingPartSummary> TopSellingParts { get; set; } = new();
}

public class PeriodRevenueSummary
{
    public string Label { get; set; } = string.Empty;
    public int SaleCount { get; set; }
    public decimal Revenue { get; set; }
    public decimal Discount { get; set; }
}

public class TopSellingPartSummary
{
    public Guid PartId { get; set; }
    public string PartName { get; set; } = string.Empty;
    public int TotalQuantity { get; set; }
    public decimal TotalRevenue { get; set; }
}
