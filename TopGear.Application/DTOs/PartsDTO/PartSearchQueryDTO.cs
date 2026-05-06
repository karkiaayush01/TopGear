using TopGear.Domain.Enums;

namespace TopGear.Application.DTOs.PartsDTO;

public class PartSearchQueryDTO
{
    public string? Search { get; set; }

    public string? Q { get; set; }

    public int Limit { get; set; } = 20;

    public VehicleType? VehicleType { get; set; }

    public decimal? MinSellingPrice { get; set; }

    public decimal? MaxSellingPrice { get; set; }

    public int? MinQuantity { get; set; }

    public int? MaxQuantity { get; set; }
}
