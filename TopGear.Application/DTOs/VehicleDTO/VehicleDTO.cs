using TopGear.Domain.Enums;

namespace TopGear.Application.DTOs.VehicleDTO;

public class VehicleDTO
{
    public Guid VehicleId { get; set; }
    public Guid CustomerId { get; set; }
    public string Make { get; set; } = null!;
    public string Model { get; set; } = null!;
    public int Year { get; set; }
    public string PlateNumber { get; set; } = null!;
    public VehicleType VehicleType { get; set; }
}
