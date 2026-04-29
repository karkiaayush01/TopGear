
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TopGear.Domain.Entities;

public class PurchaseInvoice
{
    public Guid PurchaseInvoiceId { get; set; } = Guid.NewGuid();

    public Guid VendorId { get; set; }

    public Guid CreatedBy { get; set; }

    [ForeignKey(nameof(CreatedBy))]
    public User Creator { get; set; } = null!;

    [Required]
    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(VendorId))]
    public Vendor Vendor { get; set; } = null!;
    public ICollection<PurchaseInvoiceItem> Items { get; set; } = new List<PurchaseInvoiceItem>();
}

