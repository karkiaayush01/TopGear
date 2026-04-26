using System.ComponentModel.DataAnnotations;

namespace TopGear.Application.DTOs.PurchaseInvoiceDTO;

public class CreatePurchaseInvoiceItemDTO
{
    [Required]
    public Guid PartId { get; set; }

    [Required]
    public int Quantity { get; set; }

    [Required]
    public decimal UnitPrice { get; set; }
}