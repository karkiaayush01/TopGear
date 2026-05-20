namespace TopGear.Application.DTOs.Report;

public class RegularCustomerDTO
{
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int PurchaseCount { get; set; }
    public decimal TotalSpent { get; set; }
    public DateTime LastPurchaseDate { get; set; }
}

public class HighSpenderDTO
{
    public int Rank { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal TotalSpent { get; set; }
    public int PurchaseCount { get; set; }
}

public class PendingCreditDTO
{
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int UnpaidInvoiceCount { get; set; }
    public decimal TotalAmountDue { get; set; }
    public DateTime OldestUnpaidDate { get; set; }
}
