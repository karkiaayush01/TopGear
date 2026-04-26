using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TopGear.Domain.Enums;

namespace TopGear.Domain.Entities;

public class Part
{
    public Guid PartId { get; set; } = Guid.NewGuid();

    [Required]
    public string PartName { get; set; } = null!;

    [Range(0, double.MaxValue)]
    [Required]
    public decimal PurchasePrice { get; set; }

    [Range(0, double.MaxValue)]
    public decimal SellingPrice { get; set; } = 0m;

    [Range(0, double.MaxValue)]
    public int Quantity { get; set; }


    public Guid VendorId { get; set; }

    public string Description { get; set; } = null!;
    public VehicleType VehicleType { get; set; }
    public string ImageUrl { get; set; } = null!;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(VendorId))]
    public Vendor Vendor { get; set; } = null!;

    //Purchase Price, Selling Price


    //Invoice ma partid Unit price() Quantity, Vendor Detail
}