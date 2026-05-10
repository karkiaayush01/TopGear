using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TopGear.Domain.Entities;

public class PartSale
{
    [Key]
    public Guid SaleId { get; set; } = Guid.NewGuid();

    [Required]
    public Guid CustomerId { get; set; }

    [Required]
    public Guid CreatedById { get; set; }

    public DateTime SaleDate { get; set; } = DateTime.UtcNow;

    [Range(0, double.MaxValue)]
    public decimal SubTotal { get; set; }

    [Range(0, double.MaxValue)]
    public decimal DiscountAmount { get; set; } = 0m;

    [Range(0, double.MaxValue)]
    public decimal FinalAmount { get; set; }

    public bool IsCredit { get; set; } = false;
    public bool IsPaid { get; set; } = true;
    public DateTime? PaidAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(CustomerId))]
    public User Customer { get; set; } = null!;

    [ForeignKey(nameof(CreatedById))]
    public User CreatedBy { get; set; } = null!;

    public ICollection<PartSaleItem> Items { get; set; } = new List<PartSaleItem>();
}
