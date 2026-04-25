using TopGear.Domain.Enums;

namespace TopGear.Application.DTOs.PartsDTO;

public class EditPartDTO
{
    public string? PartName { get; set; }

    public decimal? PartPrice { get; set; }

    public Guid? VendorId { get; set; }

    public string? Description { get; set; }

    public VehicleType? VehicleType { get; set; }

    public string? ImageUrl { get; set; }
}