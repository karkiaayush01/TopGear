using TopGear.Domain.Enums;

namespace TopGear.Application.DTOs.PartsDTO;

public class PartDTO
{
    public Guid PartId { get; set; }

    public string PartName { get; set; } = null!;

    public decimal PartPrice { get; set; }

    public decimal SellingPrice { get; set; }

    public int Quantity { get; set; }

    public Guid VendorId { get; set; }

    public string VendorName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public VehicleType VehicleType { get; set; }

    public string ImageUrl { get; set; } = null!;
}
