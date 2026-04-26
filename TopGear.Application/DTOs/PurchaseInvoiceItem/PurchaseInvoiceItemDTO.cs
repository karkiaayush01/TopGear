namespace TopGear.Application.DTOs.PurchaseInvoiceDTO;

public class PurchaseInvoiceItemDTO
{
    public Guid PartId { get; set; }

    public string PartName { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

}