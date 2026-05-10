namespace TopGear.Application.DTOs.PartSaleDTO;

public class PartSaleDTO
{
    public Guid SaleId { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = null!;
    public string CustomerEmail { get; set; } = null!;
    public Guid CreatedById { get; set; }
    public string CreatedByName { get; set; } = null!;
    public DateTime SaleDate { get; set; }
    public decimal SubTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public bool IsCredit { get; set; }
    public bool IsPaid { get; set; }
    public DateTime? PaidAt { get; set; }
    public List<PartSaleItemDTO> Items { get; set; } = new();
}

public class PartSaleItemDTO
{
    public Guid SaleItemId { get; set; }
    public Guid PartId { get; set; }
    public string PartName { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}
