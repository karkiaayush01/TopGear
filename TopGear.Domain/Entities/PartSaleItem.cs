using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TopGear.Domain.Entities;

public class PartSaleItem
{
    [Key]
    public Guid SaleItemId { get; set; } = Guid.NewGuid();

    [Required]
    public Guid SaleId { get; set; }

    [Required]
    public Guid PartId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Range(0, double.MaxValue)]
    public decimal UnitPrice { get; set; }

    [Range(0, double.MaxValue)]
    public decimal TotalPrice { get; set; }

    [ForeignKey(nameof(SaleId))]
    public PartSale Sale { get; set; } = null!;

    [ForeignKey(nameof(PartId))]
    public Part Part { get; set; } = null!;
}
