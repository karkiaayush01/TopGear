namespace TopGear.Application.DTOs.PurchaseInvoiceDTO;

public class PurchaseInvoiceDTO
{
    public Guid PurchaseInvoiceId { get; set; }

    public Guid VendorId { get; set; }

    public string VendorName { get; set; } = string.Empty;

    public DateTime InvoiceDate { get; set; }


    public List<PurchaseInvoiceItemDTO> Items { get; set; } = new();
}