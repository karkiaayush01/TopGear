using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TopGear.Domain.Entities;

public class PurchaseInvoiceItem
{
    public Guid PurchaseInvoiceItemId { get; set; } = Guid.NewGuid();

    public Guid PurchaseInvoiceId { get; set; }

    [ForeignKey(nameof(PurchaseInvoiceId))]
    public PurchaseInvoice PurchaseInvoice { get; set; } = null!;

    public Guid PartId { get; set; }

    [ForeignKey(nameof(PartId))]
    public Part Part { get; set; } = null!;

    [Required]
    public int Quantity { get; set; }

    [Required]
    public decimal UnitPrice { get; set; }
}