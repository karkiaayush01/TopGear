using System.ComponentModel.DataAnnotations;

namespace TopGear.Application.DTOs.PurchaseInvoiceDTO;

public class CreatePurchaseInvoiceDTO
{
    [Required]
    public Guid VendorId { get; set; }

    [Required]
    public List<CreatePurchaseInvoiceItemDTO> Items { get; set; } = new();
}